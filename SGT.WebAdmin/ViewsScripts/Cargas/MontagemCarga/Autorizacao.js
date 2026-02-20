/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />

// #region Objetos Globais do Arquivo

var _carregamentoAutorizacao;
var _carregamentoAutorizacaoDetalhe;
var _gridCarregamentoAutorizacao;

// #endregion Objetos Globais do Arquivo

// #region Classes

var CarregamentoAutorizacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.AprovacoesNecessarias = PropertyEntity({ type: types.map, val: ko.observable(""), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.AprovacoesNecessarias.getFieldDescription()), enable: ko.observable(true) });
    this.Aprovacoes = PropertyEntity({ type: types.map, val: ko.observable(""), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Aprovacoes.getFieldDescription()), enable: ko.observable(true) });
    this.DescricaoSituacao = PropertyEntity({ type: types.map, val: ko.observable(""), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DescricaoSituacao.getFieldDescription()), enable: ko.observable(true) });
    this.Reprovacoes = PropertyEntity({ type: types.map, val: ko.observable(""), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Reprovacoes.getFieldDescription()), enable: ko.observable(true) });
    this.PossuiRegras = PropertyEntity({ type: types.map, getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.ExibirAprovacao = PropertyEntity({ eventClick: exibirCarregamentoAprovacaoClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.MontagemCarga.ExibirAprovacao.getFieldDescription()), fadeVisible: ko.observable(false), visible: ko.observable(false) });
    this.ReprocessarRegras = PropertyEntity({ eventClick: reprocessarRegrasCarregamentoClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.MontagemCarga.ReprocessarRegras.getFieldDescription()) });
}

var CarregamentoAutorizacaoDetalhe = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Regra = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Regra.getFieldDescription()), val: ko.observable("") });
    this.Data = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Data.getFieldDescription()), val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Situacao.getFieldDescription()), val: ko.observable("") });
    this.Usuario = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Usuario.getFieldDescription()), val: ko.observable("") });
    this.Motivo = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Motivo.getFieldDescription()), val: ko.observable(""), visible: ko.observable(true) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadGridCarregamentoAutorizacao() {
    var opcaoDetalhes = { descricao: Localization.Resources.Cargas.MontagemCarga.Detalhes, id: "clasEditar", evento: "onclick", metodo: detalharCarregamentoAutorizacaoClick, tamanho: "20", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoDetalhes] };

    _gridCarregamentoAutorizacao = new GridView("grid-carregamento-autorizacao", "MontagemCarga/PesquisaAutorizacoes", _carregamentoAutorizacao, menuOpcoes);
    _gridCarregamentoAutorizacao.CarregarGrid();
}

function loadCarregamentoAutorizacao() {
    if (_CONFIGURACAO_TMS.UtilizarAlcadaAprovacaoCarregamento) {
        _carregamentoAutorizacao = new CarregamentoAutorizacao();
        KoBindings(_carregamentoAutorizacao, "knockoutCarregamentoAutorizacao");

        _carregamentoAutorizacaoDetalhe = new CarregamentoAutorizacaoDetalhe();
        KoBindings(_carregamentoAutorizacaoDetalhe, "knockoutCarregamentoAutorizacaoDetalhe");

        loadGridCarregamentoAutorizacao();
    }
    else {
        $("#knockoutCarregamentoAutorizacao").remove();
        $("#divModalCarregamentoAutorizacao").remove();
    }
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function detalharCarregamentoAutorizacaoClick(registroSelecionado) {
    executarReST("MontagemCarga/DetalhesAutorizacao", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_carregamentoAutorizacaoDetalhe, retorno);
                exibirModalCarregamentoAutorizacao();
            }
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Cargas.MontagemCarga.Falha, retorno.Msg);
    });
}

function exibirCarregamentoAprovacaoClick() {
    controlarExibicaoCarregamentoAutorizacao(_carregamentoAutorizacao.ExibirAprovacao.fadeVisible() == false);
}

function reprocessarRegrasCarregamentoClick() {
    executarReST("MontagemCarga/ReprocessarRegras", { Codigo: _carregamentoAutorizacao.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Cargas.MontagemCarga.Sucesso, Localization.Resources.Cargas.MontagemCarga.RegrasAprovacaoReprocessadasComSucesso);
                preencherCarregamentoAutorizacao(_carregamentoAutorizacao.Codigo.val());
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.MontagemCarga.SemRegra, retorno.Msg || Localization.Resources.Cargas.MontagemCarga.NenhumaRegraDeAprovacaoEncontrada);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Cargas.MontagemCarga.Falha, retorno.Msg);
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function limparCarregamentoAutorizacao() {
    if (_CONFIGURACAO_TMS.UtilizarAlcadaAprovacaoCarregamento) {
        LimparCampos(_carregamentoAutorizacao);
        controlarExibicaoCarregamentoAutorizacao(false);
        _carregamentoAutorizacao.ExibirAprovacao.visible(false);
        _gridCarregamentoAutorizacao.CarregarGrid();
    }
}

function preencherCarregamentoAutorizacao(codigoCarregamento) {
    if (_CONFIGURACAO_TMS.UtilizarAlcadaAprovacaoCarregamento) {
        executarReST("MontagemCarga/BuscarResumoAprovacao", { Codigo: codigoCarregamento }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    PreencherObjetoKnout(_carregamentoAutorizacao, retorno);

                    if (retorno.Data.PossuiAlcada) {
                        _carregamentoAutorizacao.ExibirAprovacao.visible(true);
                        _gridCarregamentoAutorizacao.CarregarGrid();
                    }
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.MontagemCarga.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Cargas.MontagemCarga.Falha, retorno.Msg);
        });
    }
}

// #endregion Funções Públicas

// #region Funções Privadas

function controlarExibicaoCarregamentoAutorizacao(exibir) {
    _carregamentoAutorizacao.ExibirAprovacao.fadeVisible(exibir);
    _carregamentoAutorizacao.ExibirAprovacao.text(exibir ? Localization.Resources.Cargas.MontagemCarga.OcultarAprovacao : Localization.Resources.Cargas.MontagemCarga.ExibirAprovacao);
}

function exibirModalCarregamentoAutorizacao() {
    Global.abrirModal("divModalCarregamentoAutorizacao");
    $("#divModalCarregamentoAutorizacao").one('hidden.bs.modal', function () {
        LimparCampos(_carregamentoAutorizacaoDetalhe);
    });
}

// #endregion Funções Privadas
