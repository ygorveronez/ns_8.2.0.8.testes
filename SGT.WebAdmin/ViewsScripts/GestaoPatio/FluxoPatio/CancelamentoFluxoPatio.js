/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Validacao.js" />
/// <reference path="../../Consultas/MotivoRetiradaFilaCarregamento.js" />

// #region Objetos Globais do Arquivo

var _cancelamentoFluxoPatio;
var _CRUDCancelamentoFluxoPatio;

// #endregion Objetos Globais do Arquivo

// #region Classes

var CRUDCancelamentoFluxoPatio = function () {
    this.SolicitarCancelamento = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.SolicitarCancelamento, eventClick: solicitarCancelamentoFluxoClick, visible: ko.observable(true), type: types.event, idGrid: guid() });
}

var CancelamentoFluxoPatio = function () {
    this.CodigoFluxo = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });
    this.MensagemAviso = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.AoCancelarSeraGeradoUmNovoFluxoPatio, visible: ko.observable(false) });

    this.Motivo = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Motivo.getRequiredFieldDescription(), val: ko.observable(""), maxlength: 300, required: true, getType: typesKnockout.string, enable: ko.observable(false) });
    this.RemoverVeiculoFilaCarregamento = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.GestaoPatio.FluxoPatio.RemoverVeiculoDaFilaDeCarregamento, enable: ko.observable(false), visible: _configuracaoGestaoPatio.PermiteRemoverVeiculoFilaCarregamentoAoCancelarFluxoPatio });
    this.MotivoRetiradaFilaCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.GestaoPatio.FluxoPatio.MotivoRetiradaFilaCarregamento.getRequiredFieldDescription(), idBtnSearch: guid(), visible: this.RemoverVeiculoFilaCarregamento.val, required: this.RemoverVeiculoFilaCarregamento.val, enable: ko.observable(false) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadCancelamentoFluxoPatio() {
    _cancelamentoFluxoPatio = new CancelamentoFluxoPatio();
    KoBindings(_cancelamentoFluxoPatio, "knockoutCancelamentoFluxoPatio");

    _CRUDCancelamentoFluxoPatio = new CRUDCancelamentoFluxoPatio();
    KoBindings(_CRUDCancelamentoFluxoPatio, "knockoutCRUDCancelamentoFluxoPatio");

    new BuscarMotivoRetiradaFilaCarregamento(_cancelamentoFluxoPatio.MotivoRetiradaFilaCarregamento);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function solicitarCancelamentoFluxoClick() {
    if (!ValidarCamposObrigatorios(_cancelamentoFluxoPatio)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Gerais.Geral.PreenchaOsCamposObrigatorios);
        return;
    }

    if (_cancelamentoFluxoPatio.Motivo.val().length < 20) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.GestaoPatio.FluxoPatio.MotivoPrecisaTerNoMinimoVinteCaracteres);
        return;
    }

    executarReST("FluxoPatio/CancelarFluxoPatio", RetornarObjetoPesquisa(_cancelamentoFluxoPatio), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, retorno.Msg);
                Global.fecharModal("divModalCancelamentoFluxoPatio");
                atualizarFluxoPatio();
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function exibirCancelamentoFluxoPatio(codigoFluxo) {
    _cancelamentoFluxoPatio.CodigoFluxo.val(codigoFluxo);

    controlarComponentesHabilitadosCancelamentoFluxoPatio(true);
    exibirModalCancelamentoFluxoPatio(Localization.Resources.GestaoPatio.FluxoPatio.CancelarFluxoDePatioDaCarga.format(_fluxoAtual.NumeroCarga.val()));
}

function exibirDetalhesCancelamentoFluxoPatio(codigoFluxo) {
    _cancelamentoFluxoPatio.CodigoFluxo.val(codigoFluxo);

    executarReST("FluxoPatio/BuscarCancelamentoFluxoPatio", { CodigoFluxo: _cancelamentoFluxoPatio.CodigoFluxo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_cancelamentoFluxoPatio, retorno);
                controlarComponentesHabilitadosCancelamentoFluxoPatio(false);
                exibirModalCancelamentoFluxoPatio(Localization.Resources.GestaoPatio.FluxoPatio.DetalhesDoCancelamentoDoFluxoDePatioDaCarga.format(_fluxoAtual.NumeroCarga.val()));
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

// #endregion Funções Públicas

// #region Funções Privadas

function controlarComponentesHabilitadosCancelamentoFluxoPatio(cancelamentoEmEdicao) {
    _CRUDCancelamentoFluxoPatio.SolicitarCancelamento.visible(cancelamentoEmEdicao);
    _cancelamentoFluxoPatio.MensagemAviso.visible(cancelamentoEmEdicao);
    _cancelamentoFluxoPatio.RemoverVeiculoFilaCarregamento.enable(cancelamentoEmEdicao);
    _cancelamentoFluxoPatio.Motivo.enable(cancelamentoEmEdicao);
    _cancelamentoFluxoPatio.MotivoRetiradaFilaCarregamento.enable(cancelamentoEmEdicao);
}

function exibirModalCancelamentoFluxoPatio(titulo) {
    $("#tituloModalCancelamentoFluxoPatio").text(titulo);

    Global.abrirModal('divModalCancelamentoFluxoPatio');

    $("#divModalCancelamentoFluxoPatio").one("hidden.bs.modal", function () {
        limparCamposCancelamentoFluxoPatio();
    });
}

function limparCamposCancelamentoFluxoPatio() {
    LimparCampos(_cancelamentoFluxoPatio);
}

// #endregion Funções Privadas
