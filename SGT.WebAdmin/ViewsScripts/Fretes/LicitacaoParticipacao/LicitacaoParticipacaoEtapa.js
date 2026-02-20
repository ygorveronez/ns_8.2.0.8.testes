/// <reference path="LicitacaoParticipacaoCadastro.js" />
/// <reference path="../../Enumeradores/EnumAbaLicitacaoParticipacao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoLicitacaoParticipacao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _abaAtiva;
var _etapaLicitacaoParticipacao;
var _etapaOfertaAtiva = false;
var _etapaRetornoOfertaAtiva = false;

/*
 * Declaração das Classes
 */

var EtapaLicitacaoParticipacao = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("33.33%") });

    this.EtapaInscricao = PropertyEntity({
        text: "Inscrição", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaInscricaoClick,
        step: ko.observable(1),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("Inscrição")
    });

    this.EtapaOferta = PropertyEntity({
        text: "Oferta", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaOfertaClick,
        step: ko.observable(2),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("Oferta")
    });

    this.EtapaRetornoOferta = PropertyEntity({
        text: "Retorno Oferta", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaRetornoOfertaClick,
        step: ko.observable(3),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("Retorno Oferta")
    });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadEtapaLicitacaoParticipacao() {
    _etapaLicitacaoParticipacao = new EtapaLicitacaoParticipacao();
    KoBindings(_etapaLicitacaoParticipacao, "knockoutEtapaLicitacaoParticipacao");

    EtapaInscricaoLiberada();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function etapaInscricaoClick() {
    _abaAtiva = EnumAbaLicitacaoParticipacao.Inscricao;

    controlarBotoesHabilitados();
}

function etapaOfertaClick() {
    if (_etapaOfertaAtiva) {
        _abaAtiva = EnumAbaLicitacaoParticipacao.Oferta;

        controlarBotoesHabilitados();
    }
}

function etapaRetornoOfertaClick() {
    if (_etapaRetornoOfertaAtiva) {
        _abaAtiva = EnumAbaLicitacaoParticipacao.RetornoOferta;

        controlarBotoesHabilitados();
    }
}

/*
 * Declaração das Funções Públicas
 */

function setarEtapaInicial() {
    EtapaInscricaoLiberada();

    Global.ResetarAbas();
    Global.ResetarSteps();
}

function setarEtapas() {
    Global.ResetarAbas();

    switch (_licitacaoParticipacaoCadastro.Situacao.val()) {
        case EnumSituacaoLicitacaoParticipacao.AguardandoOferta:
            EtapaOfertaLiberada();
            break;

        case EnumSituacaoLicitacaoParticipacao.AguardandoRetornoOferta:
            EtapaRetornoOfertaLiberada();
            break;

        case EnumSituacaoLicitacaoParticipacao.Cancelada:
            EtapaOfertaCancelada();
            break;

        case EnumSituacaoLicitacaoParticipacao.OfertaAceita:
            EtapaRetornoOfertaAprovada();
            break;

        case EnumSituacaoLicitacaoParticipacao.OfertaRecusada:
            EtapaRetornoOfertaReprovada();
            break;

        case EnumSituacaoLicitacaoParticipacao.Todas:
            EtapaInscricaoLiberada();
            break;
    }
}

/*
 * Declaração das Funções da Etapa Um (Inscrição)
 */

function EtapaInscricaoAprovada() {
    $("#" + _etapaLicitacaoParticipacao.EtapaInscricao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLicitacaoParticipacao.EtapaInscricao.idTab + " .step").attr("class", "step green");
}

function EtapaInscricaoLiberada() {
    $("#" + _etapaLicitacaoParticipacao.EtapaInscricao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLicitacaoParticipacao.EtapaInscricao.idTab + " .step").attr("class", "step yellow");

    EtapaOfertaDesabilitada();
}

/*
 * Declaração das Funções da Etapa Dois (Oferta)
 */

function EtapaOfertaAprovada() {
    $("#" + _etapaLicitacaoParticipacao.EtapaOferta.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLicitacaoParticipacao.EtapaOferta.idTab + " .step").attr("class", "step green");

    _etapaOfertaAtiva = true;

    EtapaInscricaoAprovada();
}

function EtapaOfertaCancelada() {
    $("#" + _etapaLicitacaoParticipacao.EtapaOferta.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLicitacaoParticipacao.EtapaOferta.idTab + " .step").attr("class", "step red");

    _etapaOfertaAtiva = true;

    EtapaInscricaoAprovada();
    EtapaRetornoOfertaDesabilitada();
}

function EtapaOfertaDesabilitada() {
    $("#" + _etapaLicitacaoParticipacao.EtapaOferta.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaLicitacaoParticipacao.EtapaOferta.idTab + " .step").attr("class", "step");

    _etapaOfertaAtiva = false;

    EtapaRetornoOfertaDesabilitada();
}

function EtapaOfertaLiberada() {
    $("#" + _etapaLicitacaoParticipacao.EtapaOferta.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLicitacaoParticipacao.EtapaOferta.idTab + " .step").attr("class", "step yellow");

    _etapaOfertaAtiva = true;

    EtapaInscricaoAprovada();
    EtapaRetornoOfertaDesabilitada();
}

/*
 * Declaração das Funções da Etapa Três (Retorno Oferta)
 */

function EtapaRetornoOfertaAprovada() {
    $("#" + _etapaLicitacaoParticipacao.EtapaRetornoOferta.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLicitacaoParticipacao.EtapaRetornoOferta.idTab + " .step").attr("class", "step green");

    _etapaRetornoOfertaAtiva = true;

    EtapaOfertaAprovada();
}

function EtapaRetornoOfertaDesabilitada() {
    $("#" + _etapaLicitacaoParticipacao.EtapaRetornoOferta.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaLicitacaoParticipacao.EtapaRetornoOferta.idTab + " .step").attr("class", "step");

    _etapaRetornoOfertaAtiva = false;
}

function EtapaRetornoOfertaLiberada() {
    $("#" + _etapaLicitacaoParticipacao.EtapaRetornoOferta.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLicitacaoParticipacao.EtapaRetornoOferta.idTab + " .step").attr("class", "step yellow");

    _etapaRetornoOfertaAtiva = true;

    EtapaOfertaAprovada();
}

function EtapaRetornoOfertaReprovada() {
    $("#" + _etapaLicitacaoParticipacao.EtapaRetornoOferta.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLicitacaoParticipacao.EtapaRetornoOferta.idTab + " .step").attr("class", "step red");

    _etapaRetornoOfertaAtiva = true;

    EtapaOfertaAprovada();
}