/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

// #region Objetos Globais do Arquivo

var _knoutCargaAlterarDataTransportador;
var _knoutInfoCarregamentoTransportador;
var _gridHorariosInfoCarregamentoTransportador;
var _gridDatasCarregamentoCarga;

// #endregion Objetos Globais do Arquivo

// #region Classes

var InfoCarregamentoTransportador = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataCarregamento = PropertyEntity({ text: "*Data Carregamento: ", required: true, getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataDisponibilidade = PropertyEntity({ text: "Data: ", enable: ko.observable(true), getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), diminuirData: diminuirDataClick, aumentarData: aumentarDataClick, idGrid: guid() });

    this.DataDisponibilidade.val.subscribe(atualizarHorarios)

    this.ConfirmarAlteracao = PropertyEntity({ type: types.event, eventClick: confirmarAlteracaoCarregamentoClick, text: "Confirmar", visible: ko.observable(true) });
}

// #endregion Classes

// #region Funções Associadas a Eventos

function aumentarDataClick() {
    definirDataDisponibilidadeTransportador(1);
}

function diminuirDataClick() {
    definirDataDisponibilidadeTransportador(-1);
}

function atualizarHorarios() {
    if (!_knoutInfoCarregamentoTransportador.DataDisponibilidade.val())
        return;

    _gridHorariosInfoCarregamentoTransportador.CarregarGrid();
}

function confirmarAlteracaoCarregamentoClick() {
    if (!_knoutInfoCarregamentoTransportador.DataCarregamento.val()) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe a data de carregamento.");
        return;
    }

    var dados = {
        Carga: _knoutInfoCarregamentoTransportador.Carga.val(),
        DataCarregamento: _knoutInfoCarregamentoTransportador.DataCarregamento.val(),
    };

    executarReST("Carga/AlterarDataCarregamento", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data != false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Data alterada com sucesso");
                _knoutCargaAlterarDataTransportador.DataCarregamento.val(_knoutInfoCarregamentoTransportador.DataCarregamento.val());
                _knoutCargaAlterarDataTransportador.PermitirSelecionarDataCarregamento.val(false);
                Global.fecharModal('divModalAlterarCarregamentoTransportador');
                limparHorarioSelecionadoTransportador();
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

function alterarDataCarregamentoClick(e) {
    _knoutCargaAlterarDataTransportador = e;

    carregarHorariosinfoCarregamento();

    _knoutInfoCarregamentoTransportador.Carga.val(e.Carga.val());
    _knoutInfoCarregamentoTransportador.DataDisponibilidade.enable(!e.BloquearTrocaDataListaHorarios.val());
    _gridHorariosInfoCarregamentoTransportador.CarregarGrid();
    
    if (!string.IsNullOrWhiteSpace(e.DataCarregamentoReal.val()))
        _knoutInfoCarregamentoTransportador.DataDisponibilidade.val(e.DataCarregamentoReal.val().substring(0, 10));
        
    Global.abrirModal('divModalAlterarCarregamentoTransportador');
}

// #endregion Funções Públicas

// #region Funções Privadas

function carregarHorariosinfoCarregamento() {
    if (_knoutInfoCarregamentoTransportador)
        return;

    _knoutInfoCarregamentoTransportador = new InfoCarregamentoTransportador();
    KoBindings(_knoutInfoCarregamentoTransportador, "knoutAlterarCarregamentoTransportador");

    var multiplaescolha = {
        basicGrid: null,
        callbackSelecionado: function (obj, data) {
            horarioSelecionadoTransportador(data);
        },
        callbackNaoSelecionado: function () {
            _knoutInfoCarregamentoTransportador.DataCarregamento.val("");
        },
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        somenteLeitura: false
    };

    _gridHorariosInfoCarregamentoTransportador = new GridView(_knoutInfoCarregamentoTransportador.DataDisponibilidade.idGrid, "JanelaCarregamentoTransportador/ObterHorariosDisponiveis", _knoutInfoCarregamentoTransportador, null, null, 10, null, false, null, multiplaescolha);
}

function definirDataDisponibilidadeTransportador(dias) {
    if (!_knoutInfoCarregamentoTransportador.DataDisponibilidade.val())
        return;

    var dataDisponibilidade = moment(_knoutInfoCarregamentoTransportador.DataDisponibilidade.val(), 'DD/MM/YYYY');

    dataDisponibilidade.add(dias, 'day');
    _knoutInfoCarregamentoTransportador.DataDisponibilidade.val(dataDisponibilidade.format('DD/MM/YYYY'));

    limparHorarioSelecionadoTransportador();
}

function horarioSelecionadoTransportador(data) {
    _gridHorariosInfoCarregamentoTransportador.AtualizarRegistrosSelecionados([data]);
    _gridHorariosInfoCarregamentoTransportador.DrawTable(true);

    var dataHoraComSegundos = _knoutInfoCarregamentoTransportador.DataDisponibilidade.val() + ' ' + data.HoraInicio;
    var dataHoraSemSegundos = dataHoraComSegundos.substring(0, 16);

    _knoutInfoCarregamentoTransportador.DataCarregamento.val(dataHoraSemSegundos);
}

function limparHorarioSelecionadoTransportador() {
    _gridHorariosInfoCarregamentoTransportador.AtualizarRegistrosSelecionados([]);
    _gridHorariosInfoCarregamentoTransportador.DrawTable();
    _knoutInfoCarregamentoTransportador.DataCarregamento.val("");
}

// #endregion Funções Privadas
