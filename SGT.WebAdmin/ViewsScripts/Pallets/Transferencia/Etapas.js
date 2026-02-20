/// <reference path="Transferencia.js" />
/// <reference path="../../Enumeradores/EnumAbaTransferenciaPallet.js" />
/// <reference path="../../Enumeradores/EnumSituacaoTransferenciaPallet.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _etapaTransferenciaPallet;
var _etapaAprovacaoAtiva = false;
var _etapaEnvioAtiva = false;
var _etapaRecebimentoAtiva = false;

/*
 * Declaração das Classes
 */

var EtapaTransferenciaPallet = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("25%") });

    this.EtapaAprovacao = PropertyEntity({
        text: "Aprovação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaAprovacaoClick,
        step: ko.observable(3),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("Aprovação")
    });

    this.EtapaEnvio = PropertyEntity({
        text: "Envio", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaEnvioClick,
        step: ko.observable(2),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("Envio")
    });

    this.EtapaRecebimento = PropertyEntity({
        text: "Recebimento", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaRecebimentoClick,
        step: ko.observable(4),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("Recebimento")
    });

    this.EtapaSolicitacao = PropertyEntity({
        text: "Solicitação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaSolicitacaoClick,
        step: ko.observable(1),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("Solicitação")
    });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadEtapaTransferenciaPallet() {
    _etapaTransferenciaPallet = new EtapaTransferenciaPallet();
    KoBindings(_etapaTransferenciaPallet, "knockoutEtapaTransferenciaPallet");

    EtapaSolicitacaoLiberada();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function etapaAprovacaoClick() {
    if (_etapaAprovacaoAtiva) {
        _abaAtiva = EnumAbaTransferenciaPallet.Aprovacao;

        controlarBotoesHabilitados();
    }
}

function etapaEnvioClick() {
    if (_etapaEnvioAtiva) {
        _abaAtiva = EnumAbaTransferenciaPallet.Envio;

        controlarBotoesHabilitados();
    }
}

function etapaRecebimentoClick() {
    if (_etapaRecebimentoAtiva) {
        _abaAtiva = EnumAbaTransferenciaPallet.Recebimento;

        controlarBotoesHabilitados();
    }
}

function etapaSolicitacaoClick() {
    _abaAtiva = EnumAbaTransferenciaPallet.Solicitacao;

    controlarBotoesHabilitados();
}

/*
 * Declaração das Funções Públicas
 */

function EtapaSemRegra() {
    exibirMensagem(tipoMensagem.aviso, "Regras da etapa", "Nenhuma regra encontrada. A solicitação permanecerá aguardando autorização.");

    _CRUDTransferenciaPallet.ReprocessarRegras.visible(true);
}

function setarEtapaInicial() {
    EtapaSolicitacaoLiberada();

    $("#" + _etapaTransferenciaPallet.EtapaSolicitacao.idTab).click();
    $("#" + _etapaTransferenciaPallet.EtapaSolicitacao.idTab).tab("show");
}

function setarEtapas() {
    $("#" + _etapaTransferenciaPallet.EtapaSolicitacao.idTab).click();
    $("#" + _etapaTransferenciaPallet.EtapaSolicitacao.idTab).tab("show");
    switch (_transferenciaPallet.Situacao.val()) {
        case EnumSituacaoTransferenciaPallet.AguardandoAprovacao:
            EtapaAprovacaoAguardando();
            break;

        case EnumSituacaoTransferenciaPallet.AguardandoEnvio:
            EtapaEnvioLiberada();
            break;

        case EnumSituacaoTransferenciaPallet.AguardandoRecebimento:
            EtapaRecebimentoLiberada();
            break;

        case EnumSituacaoTransferenciaPallet.AprovacaoRejeitada:
            EtapaAprovacaoRejeitada();
            break;

        case EnumSituacaoTransferenciaPallet.EnvioCancelado:
            EtapaSolicitacaoCancelada();
            break;

        case EnumSituacaoTransferenciaPallet.Finalizada:
            EtapaRecebimentoAprovada();
            break;

        case EnumSituacaoTransferenciaPallet.Todas:
            EtapaSolicitacaoLiberada();
            break;

        case EnumSituacaoTransferenciaPallet.SemRegraAprovacao:
            EtapaAprovacaoSemRegra();
            break;
    }
}

/*
 * Declaração das Funções da Etapa Um (Solicitação)
 */

function EtapaSolicitacaoAprovada() {
    $("#" + _etapaTransferenciaPallet.EtapaSolicitacao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaTransferenciaPallet.EtapaSolicitacao.idTab + " .step").attr("class", "step green");
}

function EtapaSolicitacaoLiberada() {
    $("#" + _etapaTransferenciaPallet.EtapaSolicitacao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaTransferenciaPallet.EtapaSolicitacao.idTab + " .step").attr("class", "step yellow");

    EtapaEnvioDesabilitada();
}

function EtapaSolicitacaoCancelada() {
    $("#" + _etapaTransferenciaPallet.EtapaSolicitacao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaTransferenciaPallet.EtapaSolicitacao.idTab + " .step").attr("class", "step red");

    EtapaEnvioDesabilitada();
}

/*
 * Declaração das Funções da Etapa Dois (Envio)
 */

function EtapaEnvioAprovada() {
    $("#" + _etapaTransferenciaPallet.EtapaEnvio.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaTransferenciaPallet.EtapaEnvio.idTab + " .step").attr("class", "step green");

    _etapaEnvioAtiva = true;

    EtapaSolicitacaoAprovada();
}

function EtapaEnvioDesabilitada() {
    $("#" + _etapaTransferenciaPallet.EtapaEnvio.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaTransferenciaPallet.EtapaEnvio.idTab + " .step").attr("class", "step");

    _etapaEnvioAtiva = false;

    EtapaAprovacaoDesabilitada();
}

function EtapaEnvioLiberada() {
    $("#" + _etapaTransferenciaPallet.EtapaEnvio.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaTransferenciaPallet.EtapaEnvio.idTab + " .step").attr("class", "step yellow");

    _etapaEnvioAtiva = true;

    EtapaSolicitacaoAprovada();
    EtapaAprovacaoDesabilitada();
}

/*
 * Declaração das Funções da Etapa Três (Aprovação)
 */

function EtapaAprovacaoAguardando() {
    $("#" + _etapaTransferenciaPallet.EtapaAprovacao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaTransferenciaPallet.EtapaAprovacao.idTab + " .step").attr("class", "step yellow");

    _etapaAprovacaoAtiva = true;

    EtapaEnvioAprovada();
    EtapaRecebimentoDesabilitada();
}

function EtapaAprovacaoAprovada() {
    $("#" + _etapaTransferenciaPallet.EtapaAprovacao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaTransferenciaPallet.EtapaAprovacao.idTab + " .step").attr("class", "step green");

    _etapaAprovacaoAtiva = true;

    EtapaEnvioAprovada();
}

function EtapaAprovacaoDesabilitada() {
    $("#" + _etapaTransferenciaPallet.EtapaAprovacao.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaTransferenciaPallet.EtapaAprovacao.idTab + " .step").attr("class", "step");

    _etapaAprovacaoAtiva = false;

    EtapaRecebimentoDesabilitada();
}

function EtapaAprovacaoRejeitada() {
    $("#" + _etapaTransferenciaPallet.EtapaAprovacao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaTransferenciaPallet.EtapaAprovacao.idTab + " .step").attr("class", "step red");

    _etapaAprovacaoAtiva = true;

    EtapaEnvioAprovada();
    EtapaRecebimentoDesabilitada();
}

function EtapaAprovacaoSemRegra() {
    $("#" + _etapaTransferenciaPallet.EtapaAprovacao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaTransferenciaPallet.EtapaAprovacao.idTab + " .step").attr("class", "step red");

    _etapaAprovacaoAtiva = true;

    EtapaEnvioAprovada();
    EtapaRecebimentoDesabilitada();
    EtapaSemRegra();
}

/*
 * Declaração das Funções da Etapa Quatro (Recebimento)
 */

function EtapaRecebimentoAprovada() {
    $("#" + _etapaTransferenciaPallet.EtapaRecebimento.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaTransferenciaPallet.EtapaRecebimento.idTab + " .step").attr("class", "step green");

    _etapaRecebimentoAtiva = true;

    EtapaAprovacaoAprovada();
}

function EtapaRecebimentoDesabilitada() {
    $("#" + _etapaTransferenciaPallet.EtapaRecebimento.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaTransferenciaPallet.EtapaRecebimento.idTab + " .step").attr("class", "step");

    _etapaRecebimentoAtiva = false;
}

function EtapaRecebimentoLiberada() {
    $("#" + _etapaTransferenciaPallet.EtapaRecebimento.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaTransferenciaPallet.EtapaRecebimento.idTab + " .step").attr("class", "step yellow");

    _etapaRecebimentoAtiva = true;

    EtapaAprovacaoAprovada();
}