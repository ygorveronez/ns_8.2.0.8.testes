/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

// #region Objetos Globais do Arquivo

var _janelaCarregamentoTransportadorDataDescarregamentoContainer;

// #endregion Objetos Globais do Arquivo

// #region Classes

var JanelaCarregamentoTransportadorDataDescarregamento = function (dados) {
    var self = this;

    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CentroDescarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.DataDescarregamento = PropertyEntity({ getType: typesKnockout.dateTime });
    this.DataDescarregamentoAgendada = PropertyEntity({ });
    this.DataDisponibilidade = PropertyEntity({ text: "Data: ", enable: ko.observable(true), visible: ko.observable(true), getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), diminuirData: diminuirDataDescarregamentoClick, aumentarData: aumentarDataDescarregamentoClick, idGrid: guid() });
    this.GridHorarios;

    this.DataDisponibilidade.val.subscribe(function () { atualizarHorariosDescarregamentosDisponiveis(self); });
  
    PreencherObjetoKnout(this, { Data: dados });
};

var JanelaCarregamentoTransportadorDataDescarregamentoContainer = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.PermitirEdicao = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.ListaDataDescarregamento = ko.observableArray([]);

    this.ConfirmarAlteracao = PropertyEntity({ type: types.event, eventClick: confirmarAlteracaoJanelaCarregamentoTransportadorDataDescarregamentoClick, text: "Confirmar", visible: ko.observable(true) });
};

// #endregion Classes

// #region Funções de Inicialização

function loadJanelaCarregamentoTransportadorDataDescarregamento() {
    _janelaCarregamentoTransportadorDataDescarregamentoContainer = new JanelaCarregamentoTransportadorDataDescarregamentoContainer();
    KoBindings(_janelaCarregamentoTransportadorDataDescarregamentoContainer, "knoutAlterarDescarregamentoTransportador");
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function confirmarAlteracaoJanelaCarregamentoTransportadorDataDescarregamentoClick() {
    var listaDataDescarregamento = _janelaCarregamentoTransportadorDataDescarregamentoContainer.ListaDataDescarregamento().slice();
    var listaDataDescarregamentoAlterar = [];

    for (var i = 0; i < listaDataDescarregamento.length; i++) {
        var knoutDataDescarregamento = listaDataDescarregamento[i];

        if (!knoutDataDescarregamento.DataDescarregamento.val()) {
            exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe a data do centro de descarregamento " + knoutDataDescarregamento.CentroDescarregamento.val() + ".");
            return;
        }

        listaDataDescarregamentoAlterar.push({
            Carga: knoutDataDescarregamento.Carga.val(),
            CentroDescarregamento: knoutDataDescarregamento.CentroDescarregamento.codEntity(),
            DataDescarregamento: knoutDataDescarregamento.DataDescarregamento.val()
        });
    }

    var dados = {
        Codigo: _janelaCarregamentoTransportadorDataDescarregamentoContainer.Codigo.val(),
        DatasDescarregamento: JSON.stringify(listaDataDescarregamentoAlterar)
    };

    executarReST("JanelaCarregamentoTransportador/AlterarHorariosDescarregamentos", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data != false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Datas alteradas com sucesso");
                Global.fecharModal('divModalAlterarDescarregamentoTransportador');

                var cargas = _pesquisaJanelaCarregamentoTransportador.Cargas();

                for (var i = 0; i < cargas.length; i++) {
                    if (cargas[i].Codigo.val() == retorno.Data.Codigo) {
                        AdicionarCarga(retorno.Data, i);
                        break;
                    }
                }
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg, 20000);
        }
        else
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function alterarJanelaCarregamentoTransportadorDataDescarregamento(janelaTransportadorSelecionada) {
    _janelaCarregamentoTransportadorDataDescarregamentoContainer.Codigo.val(janelaTransportadorSelecionada.CodigoJanelaCarregamentoTransportador.val());
    _janelaCarregamentoTransportadorDataDescarregamentoContainer.PermitirEdicao.val(true);

    preencherJanelaCarregamentoTransportadorDataDescarregamento(janelaTransportadorSelecionada);
}

function exibirJanelaCarregamentoTransportadorDataDescarregamento(janelaTransportadorSelecionada) {
    _janelaCarregamentoTransportadorDataDescarregamentoContainer.Codigo.val(janelaTransportadorSelecionada.CodigoJanelaCarregamentoTransportador.val());
    _janelaCarregamentoTransportadorDataDescarregamentoContainer.PermitirEdicao.val(false);

    preencherJanelaCarregamentoTransportadorDataDescarregamento(janelaTransportadorSelecionada);
}

// #endregion Funções Públicas

// #region Funções Privadas

function atualizarHorariosDescarregamentosDisponiveis(knoutSelecionado) {
    if (!knoutSelecionado.DataDisponibilidade.val())
        return;

    knoutSelecionado.GridHorarios.CarregarGrid();
}

function aumentarDataDescarregamentoClick(knoutSelecionado) {
    definirDataDisponibilidadeDescarregamento(knoutSelecionado, 1);
}

function criarGridHorariosDescarregamentosDisponiveis(knoutSelecionado) {
    var multiplaEscolha = {
        basicGrid: null,
        callbackSelecionado: function (obj, registroSelecionado) {
            horarioDescarregamentoSelecionado(knoutSelecionado, registroSelecionado);
        },
        callbackNaoSelecionado: function () {
            knoutSelecionado.DataDescarregamento.val("");
        },
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        somenteLeitura: false
    };

    knoutSelecionado.GridHorarios = new GridView(knoutSelecionado.DataDisponibilidade.idGrid, "JanelaCarregamentoTransportador/ObterHorariosDisponiveis", knoutSelecionado, null, null, 10, null, false, null, multiplaEscolha);
    knoutSelecionado.GridHorarios.CarregarGrid();
}

function definirDataDisponibilidadeDescarregamento(knoutSelecionado, dias) {
    if (!knoutSelecionado.DataDisponibilidade.val())
        return;

    var dataDisponibilidade = moment(knoutSelecionado.DataDisponibilidade.val(), 'DD/MM/YYYY');

    dataDisponibilidade.add(dias, 'day');
    knoutSelecionado.DataDisponibilidade.val(dataDisponibilidade.format('DD/MM/YYYY'));

    limparHorarioDescarregamentoSelecionado(knoutSelecionado);
}

function diminuirDataDescarregamentoClick(knoutSelecionado) {
    definirDataDisponibilidadeDescarregamento(knoutSelecionado, -1);
}

function horarioDescarregamentoSelecionado(knoutSelecionado, registroSelecionado) {
    knoutSelecionado.GridHorarios.AtualizarRegistrosSelecionados([registroSelecionado]);
    knoutSelecionado.GridHorarios.DrawTable(true);

    var dataHoraComSegundos = knoutSelecionado.DataDisponibilidade.val() + ' ' + registroSelecionado.HoraInicio;
    var dataHoraSemSegundos = dataHoraComSegundos.substring(0, 16);

    knoutSelecionado.DataDescarregamento.val(dataHoraSemSegundos);
}

function limparHorarioDescarregamentoSelecionado(knoutSelecionado) {
    knoutSelecionado.GridHorarios.AtualizarRegistrosSelecionados([]);
    knoutSelecionado.GridHorarios.DrawTable();
    knoutSelecionado.DataDescarregamento.val('');
}

function limparJanelaCarregamentoTransportadorDataDescarregamento() {
    var listaDataDescarregamento = _janelaCarregamentoTransportadorDataDescarregamentoContainer.ListaDataDescarregamento.slice();

    for (var i = 0; i < listaDataDescarregamento.length; i++)
        listaDataDescarregamento[i].GridHorarios.Destroy();

    _janelaCarregamentoTransportadorDataDescarregamentoContainer.ListaDataDescarregamento.removeAll();
}

function preencherJanelaCarregamentoTransportadorDataDescarregamento(janelaTransportadorSelecionada) {
    limparJanelaCarregamentoTransportadorDataDescarregamento();

    for (var i = 0; i < janelaTransportadorSelecionada.Dados.DadosDescarregamentos.length; i++) {
        var knoutDataDescarregamento = new JanelaCarregamentoTransportadorDataDescarregamento(janelaTransportadorSelecionada.Dados.DadosDescarregamentos[i]);

        _janelaCarregamentoTransportadorDataDescarregamentoContainer.ListaDataDescarregamento.push(knoutDataDescarregamento);

        criarGridHorariosDescarregamentosDisponiveis(knoutDataDescarregamento);
    }

    var carouselAlteracaoDataDescarregamento = document.querySelector('#carousel-alteracao-data-descarregamento');
    new bootstrap.Carousel(carouselAlteracaoDataDescarregamento, { interval: false });
    Global.abrirModal('divModalAlterarDescarregamentoTransportador');
}

// #endregion Funções Privadas
