/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="Container.js" />

// #region Objetos Globais do Arquivo

var _preCargaCancelamento;
var _preCargaCancelamentoMassivo;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PreCargaCancelamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Justificativa = PropertyEntity({ val: ko.observable(""), text: Localization.Resources.Cargas.Carga.JustificativaCancelamento.getRequiredFieldDescription(), visible: ko.observable(true) });
    this.Motivo = PropertyEntity({ val: ko.observable(""), text: Localization.Resources.Cargas.Carga.MotivoDoCancelamento.getFieldDescription(), visible: ko.observable(true) });

    this.Confirmar = PropertyEntity({ eventClick: confirmarCancelamentoPreCargaClick, type: types.event, text: Localization.Resources.Cargas.Carga.Confirmar, visible: ko.observable(true) });
}

var PreCargaCancelamentoMassivo = function () {
    this.Justificativa = PropertyEntity({ val: ko.observable(""), text: Localization.Resources.Cargas.Carga.JustificativaCancelamento.getRequiredFieldDescription(), visible: ko.observable(true) });
    this.Motivo = PropertyEntity({ val: ko.observable(""), text: Localization.Resources.Cargas.Carga.MotivoDoCancelamento, visible: ko.observable(true) });

    this.Confirmar = PropertyEntity({ eventClick: confirmarCancelamentoMassivoPreCargaClick, type: types.event, text: Localization.Resources.Cargas.Carga.Confirmar, visible: ko.observable(true) });
}

var PreCargaObservacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Observacao = PropertyEntity({ val: ko.observable(""), text: Localization.Resources.Cargas.Carga.Observacao.getFieldDescription(), visible: true });

    this.Confirmar = PropertyEntity({ eventClick: confirmarObservacaoPreCargaClick, type: types.event, text: Localization.Resources.Cargas.Carga.Confirmar, visible: ko.observable(true) });
}

var PreCargaAlterarDataPlanejamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Data = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataPrePlanejamento.getRequiredFieldDescription(), getType: typesKnockout.dateTime, required: true, visible: true });
    this.DataPrevisaoEntrega = PropertyEntity({ text: "Data de Previsão de Entrega:", getType: typesKnockout.dateTime, required: true, visible: true });
    this.Motivo = PropertyEntity({ val: ko.observable(""), text: Localization.Resources.Cargas.Carga.Motivo.getRequiredFieldDescription(), visible: ko.observable(true) });

    this.Confirmar = PropertyEntity({ eventClick: confirmarAlterarDataPlanejamentoClick, type: types.event, text: Localization.Resources.Cargas.Carga.Confirmar, visible: ko.observable(true) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadPreCargaDados() {
    _preCargaCancelamento = new PreCargaCancelamento();
    KoBindings(_preCargaCancelamento, "knockoutPreCargaCancelamento");

    _preCargaCancelamentoMassivo = new PreCargaCancelamentoMassivo();
    KoBindings(_preCargaCancelamentoMassivo, "knockoutPreCargaCancelamentoMassivo");

    _preCargaObservacao = new PreCargaObservacao();
    KoBindings(_preCargaObservacao, "knoutObservacaoPreCarga");

    _preCargaAlterarDataPlanejamento = new PreCargaAlterarDataPlanejamento();
    KoBindings(_preCargaAlterarDataPlanejamento, "knoutAlterarDataPlanejamento");
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function confirmarCancelamentoMassivoPreCargaClick() {
    var dadosPreCargasSelecionadas = obterDadosPreCargasSelecionadas();

    if ((dadosPreCargasSelecionadas.registrosSelecionados.length == 0) && !dadosPreCargasSelecionadas.selecionarTodos)
        return exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.Carga.CamposObrigatorios, Localization.Resources.Cargas.Carga.NenhumPrePlanejamentoSelecionado);

    if (_preCargaCancelamentoMassivo.Justificativa.val().length < 20)
        return exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.Carga.CamposObrigatorios, Localization.Resources.Cargas.Carga.AJustificativaDeveConterMaisDeVinteCaracteres);

    var dados = RetornarObjetoPesquisa(_pesquisaPreCarga);

    dados.SelecionarTodos = dadosPreCargasSelecionadas.selecionarTodos;
    dados.ItensSelecionados = JSON.stringify(dadosPreCargasSelecionadas.registrosSelecionados);
    dados.ItensNaoSelecionados = JSON.stringify(dadosPreCargasSelecionadas.registrosNaoSelecionados);
    dados.Justificativa = _preCargaCancelamentoMassivo.Justificativa.val()
    dados.Motivo = _preCargaCancelamentoMassivo.Motivo.val()

    exibirConfirmacao(Localization.Resources.Cargas.Carga.Confirmacao, Localization.Resources.Cargas.Carga.VoceTemCertezaQueDesejaCancelaPrePlanejamentos, function () {
        executarReST("PreCarga/CancelamentoMassivo", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data.PreCargas.length == 0)
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Cargas.Carga.Sucesso, Localization.Resources.Cargas.Carga.CanceladoSucesso);
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.Carga.Falha, Localization.Resources.Cargas.Carga.OcorreuFalhaCancelarPrePlanejamentos + retorno.Data.PreCargas.join(', '), 5000);

                fecharModalPreCargaCancelamentoMassivo();
                recarregarPreCargas();
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Cargas.Carga.Falha, retorno.Msg);
        }, null);
    });
}

function confirmarCancelamentoPreCargaClick() {
    if (_preCargaCancelamento.Justificativa.val().length < 20)
        return exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.Carga.CamposObrigatorios, Localization.Resources.Cargas.Carga.AJustificativaDeveConterMaisDeVinteCaracteres);

    exibirConfirmacao(Localization.Resources.Cargas.Carga.Confirmacao, Localization.Resources.Cargas.Carga.CertezaCancelarPrePlanejamento, function () {
        executarReST("PreCarga/CancelarOperacao", RetornarObjetoPesquisa(_preCargaCancelamento), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Cargas.Carga.Sucesso, Localization.Resources.Cargas.Carga.CanceladoSucesso);
                    recarregarPreCargas();
                    fecharModalPreCargaCancelamento();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.Carga.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Cargas.Carga.Falha, retorno.Msg);
        });
    });
}

function confirmarAlterarDataPlanejamentoClick() {
    if (_preCargaAlterarDataPlanejamento.Motivo.val().length < 10)
        return exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.Carga.CamposObrigatorios, Localization.Resources.Cargas.Carga.MotivoMaisDezCaracteres);

    executarReST("PreCarga/AlterarDataPlanejamento", RetornarObjetoPesquisa(_preCargaAlterarDataPlanejamento), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Cargas.Carga.Sucesso, Localization.Resources.Cargas.Carga.DataAlteradaComSucesso);
                recarregarPreCargas();
                fecharModalPreCargaAlterarDataPlanejamento();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.Carga.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Cargas.Carga.Falha, retorno.Msg);
    });
}

function confirmarObservacaoPreCargaClick() {
    if (_preCargaObservacao.Observacao.val().length < 10)
        return exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.Carga.CamposObrigatorios, Localization.Resources.Cargas.Carga.AJustificativaMaisDezCaracteres);

    executarReST("PreCarga/AdicionarObservacao", RetornarObjetoPesquisa(_preCargaObservacao), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Cargas.Carga.Sucesso, Localization.Resources.Cargas.Carga.ObservacaoComSucesso);
                recarregarPreCargas();
                fecharModalPreCargaObservacao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.Carga.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Cargas.Carga.Falha, retorno.Msg);
    });
}


// #endregion Funções Associadas a Eventos

// #region Métodos Públicos

function exibirModalPreCargaCancelamento(codigoPreCarga) {
    _preCargaCancelamento.Codigo.val(codigoPreCarga);

    Global.abrirModal('divModalPreCargaCancelamento');
    $("#divModalPreCargaCancelamento").one('hidden.bs.modal', function () {
        LimparCampos(_preCargaCancelamento);
    });
}

function exibirModalPreCargaCancelamentoMassivo() {
    Global.abrirModal('divModalPreCargaCancelamentoMassivo');
    $("#divModalPreCargaCancelamentoMassivo").one('hidden.bs.modal', function () {
        LimparCampos(_preCargaCancelamentoMassivo);
    });
}

function exibirModalObservacaoClick(registroSelecionado) {
    _preCargaObservacao.Observacao.val(registroSelecionado.ObservacaoDescricao);
    _preCargaObservacao.Codigo.val(registroSelecionado.Codigo);
    Global.abrirModal('divModalObservacao');
    $("#divModalObservacao").one('hidden.bs.modal', function () {
        LimparCampos(_preCargaObservacao);
    });
}

function exibirModalAlterarDataPlanejamento(registroSelecionado) {
    _preCargaAlterarDataPlanejamento.Data.val(registroSelecionado.DataPrevisaoEntrega);
    _preCargaAlterarDataPlanejamento.Data.val(registroSelecionado.PrevisaoEntrega);
    _preCargaAlterarDataPlanejamento.Motivo.val(registroSelecionado.MotivoAlteracaoData);
    _preCargaAlterarDataPlanejamento.Codigo.val(registroSelecionado.Codigo);
    Global.abrirModal('divModalAlterarDataPlanejamento');
    $("#divModalAlterarDataPlanejamento").one('hidden.bs.modal', function () {
        LimparCampos(_preCargaAlterarDataPlanejamento);
    });
}


// #endregion Métodos Públicos

// #region Métodos Privados

function fecharModalPreCargaCancelamento() {
    Global.fecharModal("divModalPreCargaCancelamento");
}

function fecharModalPreCargaCancelamentoMassivo() {
    Global.fecharModal("divModalPreCargaCancelamentoMassivo");
}

function fecharModalPreCargaObservacao() {
    Global.fecharModal("divModalObservacao");
}

function fecharModalPreCargaAlterarDataPlanejamento() {
    Global.fecharModal("divModalAlterarDataPlanejamento");
}

// #endregion Métodos Privados
