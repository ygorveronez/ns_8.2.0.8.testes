/// <reference path="reforma.js" />
/// <reference path="../../Enumeradores/EnumAbaReformaPallet.js" />
/// <reference path="../../Enumeradores/EnumSituacaoReformaPallet.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _etapaReformaPallet;
var _etapaNfeSaidaAtiva = false;
var _etapaRetornoAtiva = false;

/*
 * Declaração das Classes
 */

var EtapaReformaPallet = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("33.33%") });

    this.EtapaEnvio = PropertyEntity({
        text: "Envio para Reforma", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaEnvioClick,
        step: ko.observable(1),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("Envio para Reforma")
    });

    this.EtapaNfeSaida = PropertyEntity({
        text: "Nf-e Saída", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaNfeSaidaClick,
        step: ko.observable(2),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("Nf-e Saída")
    });

    this.EtapaRetorno = PropertyEntity({
        text: "Retorno Reforma", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaRetornoClick,
        step: ko.observable(3),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("Retorno Reforma")
    });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadEtapaReformaPallet() {
    _etapaReformaPallet = new EtapaReformaPallet();
    KoBindings(_etapaReformaPallet, "knockoutEtapaReformaPallet");

    EtapaEnvioLiberada();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function etapaEnvioClick() {
    _abaAtiva = EnumAbaReformaPallet.Envio;

    controlarBotoesHabilitados();
}

function etapaNfeSaidaClick() {
    if (_etapaNfeSaidaAtiva) {
        _abaAtiva = EnumAbaReformaPallet.NfeSaida;

        controlarBotoesHabilitados();
    }
}

function etapaRetornoClick() {
    if (_etapaRetornoAtiva) {
        _abaAtiva = EnumAbaReformaPallet.Retorno;

        controlarBotoesHabilitados();
    }
}

/*
 * Declaração das Funções Públicas
 */

function setarEtapaInicial() {
    EtapaEnvioLiberada();

    $("#" + _etapaReformaPallet.EtapaEnvio.idTab).click();
}

function setarEtapas() {
    $("#" + _etapaReformaPallet.EtapaEnvio.idTab).click();

    switch (_reformaPallet.Situacao.val()) {
        case EnumSituacaoReformaPallet.AguardandoNfeSaida:
            EtapaNfeSaidaLiberada();
            break;

        case EnumSituacaoReformaPallet.AguardandoRetorno:
            EtapaRetornoLiberada();
            break;

        case EnumSituacaoReformaPallet.CanceladaNfeSaida:
            EtapaEnvioCancelada();
            break;

        case EnumSituacaoReformaPallet.CanceladaRetorno:
            EtapaNfeSaidaCancelada();
            break;

        case EnumSituacaoReformaPallet.Finalizada:
            EtapaRetornoAprovada();
            break;

        case EnumSituacaoReformaPallet.Todas:
            EtapaEnvioLiberada();
            break;
    }
}

/*
 * Declaração das Funções da Etapa Um (Envio)
 */

function EtapaEnvioAprovada() {
    $("#" + _etapaReformaPallet.EtapaEnvio.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaReformaPallet.EtapaEnvio.idTab + " .step").attr("class", "step green");
}

function EtapaEnvioCancelada() {
    $("#" + _etapaReformaPallet.EtapaEnvio.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaReformaPallet.EtapaEnvio.idTab + " .step").attr("class", "step red");

    EtapaNfeSaidaDesabilitada();
}

function EtapaEnvioLiberada() {
    $("#" + _etapaReformaPallet.EtapaEnvio.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaReformaPallet.EtapaEnvio.idTab + " .step").attr("class", "step yellow");

    EtapaNfeSaidaDesabilitada();
}

/*
 * Declaração das Funções da Etapa Dois (Nf-e Saída)
 */

function EtapaNfeSaidaAprovada() {
    $("#" + _etapaReformaPallet.EtapaNfeSaida.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaReformaPallet.EtapaNfeSaida.idTab + " .step").attr("class", "step green");

    _etapaNfeSaidaAtiva = true;

    EtapaEnvioAprovada();
}

function EtapaNfeSaidaCancelada() {
    $("#" + _etapaReformaPallet.EtapaNfeSaida.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaReformaPallet.EtapaNfeSaida.idTab + " .step").attr("class", "step red");

    _etapaNfeSaidaAtiva = true;

    EtapaEnvioAprovada();
    EtapaRetornoDesabilitada();
}

function EtapaNfeSaidaDesabilitada() {
    $("#" + _etapaReformaPallet.EtapaNfeSaida.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaReformaPallet.EtapaNfeSaida.idTab + " .step").attr("class", "step");

    _etapaNfeSaidaAtiva = false;

    EtapaRetornoDesabilitada();
}

function EtapaNfeSaidaLiberada() {
    $("#" + _etapaReformaPallet.EtapaNfeSaida.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaReformaPallet.EtapaNfeSaida.idTab + " .step").attr("class", "step yellow");

    _etapaNfeSaidaAtiva = true;

    EtapaEnvioAprovada();
    EtapaRetornoDesabilitada();
}

/*
 * Declaração das Funções da Etapa Três (Retorno)
 */

function EtapaRetornoAprovada() {
    $("#" + _etapaReformaPallet.EtapaRetorno.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaReformaPallet.EtapaRetorno.idTab + " .step").attr("class", "step green");

    _etapaRetornoAtiva = true;

    EtapaNfeSaidaAprovada();
}

function EtapaRetornoDesabilitada() {
    $("#" + _etapaReformaPallet.EtapaRetorno.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaReformaPallet.EtapaRetorno.idTab + " .step").attr("class", "step");

    _etapaRetornoAtiva = false;
}

function EtapaRetornoLiberada() {
    $("#" + _etapaReformaPallet.EtapaRetorno.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaReformaPallet.EtapaRetorno.idTab + " .step").attr("class", "step yellow");

    _etapaRetornoAtiva = true;

    EtapaNfeSaidaAprovada();
}