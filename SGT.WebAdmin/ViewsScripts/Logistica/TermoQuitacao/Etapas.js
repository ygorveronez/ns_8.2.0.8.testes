/// <reference path="TermoQuitacao.js" />
/// <reference path="../../Enumeradores/EnumAbaTermoQuitacao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoTermoQuitacao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _etapaTermoQuitacao;
var _etapaAprovacaoAtiva = false;
var _etapaDadosAceiteTransportadorAtiva = false;

/*
 * Declaração das Classes
 */

var EtapaTermoQuitacao = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("33.3333%") });

    this.EtapaAprovacao = PropertyEntity({
        text: "Aprovação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaAprovacaoClick,
        step: ko.observable(3),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("Aprovação")
    });

    this.EtapaDadosAceiteTransportador = PropertyEntity({
        text: "Aceite do Transportador", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaDadosAceiteTransportadorClick,
        step: ko.observable(2),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("Dados do Aceite do Transportador")
    });

    this.EtapaDadosTermoQuitacao = PropertyEntity({
        text: "Dados do Termo", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaDadosTermoQuitacaoClick,
        step: ko.observable(1),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("Dados do Termo de Quitação")
    });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadEtapaTermoQuitacao() {
    _etapaTermoQuitacao = new EtapaTermoQuitacao();
    KoBindings(_etapaTermoQuitacao, "knockoutEtapaTermoQuitacao");

    EtapaDadosTermoQuitacaoLiberada();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function etapaAprovacaoClick() {
    if (_etapaAprovacaoAtiva) {
        _abaAtiva = EnumAbaTermoQuitacao.Aprovacao;

        controlarBotoesHabilitados();
    }
}

function etapaDadosAceiteTransportadorClick() {
    if (_etapaDadosAceiteTransportadorAtiva) {
        _abaAtiva = EnumAbaTermoQuitacao.DadosAceiteTransportador;

        controlarBotoesHabilitados();
    }
}

function etapaDadosTermoQuitacaoClick() {
    _abaAtiva = EnumAbaTermoQuitacao.DadosTermo;

    controlarBotoesHabilitados();
}

/*
 * Declaração das Funções Públicas
 */

function EtapaSemRegra() {
    exibirMensagem(tipoMensagem.aviso, "Regras da etapa", "Nenhuma regra encontrada. A solicitação permanecerá aguardando autorização.");

    _CRUDTermoQuitacao.ReprocessarRegras.visible(true);
}

function setarEtapaInicial() {
    EtapaDadosTermoQuitacaoLiberada();

    $("#" + _etapaTermoQuitacao.EtapaDadosTermoQuitacao.idTab).click();
    $("#" + _etapaTermoQuitacao.EtapaDadosTermoQuitacao.idTab).tab("show");
}

function setarEtapas() {
    $("#" + _etapaTermoQuitacao.EtapaDadosTermoQuitacao.idTab).click();
    $("#" + _etapaTermoQuitacao.EtapaDadosTermoQuitacao.idTab).tab("show");
    switch (_termoQuitacao.Situacao.val()) {
        case EnumSituacaoTermoQuitacao.AceiteTransportadorRejeitado:
            EtapaDadosAceiteTransportadorRejeitada();
            break;

        case EnumSituacaoTermoQuitacao.AguardandoAceiteTransportador:
            EtapaDadosAceiteTransportadorAguardando();
            break;

        case EnumSituacaoTermoQuitacao.AguardandoAprovacao:
            EtapaAprovacaoAguardando();
            break;

        case EnumSituacaoTermoQuitacao.AprovacaoRejeitada:
            EtapaAprovacaoRejeitada();
            break;

        case EnumSituacaoTermoQuitacao.Finalizado:
            EtapaAprovacaoAprovada();
            break;

        case EnumSituacaoTermoQuitacao.Todas:
            EtapaDadosTermoQuitacaoLiberada();
            break;

        case EnumSituacaoTermoQuitacao.SemRegraAprovacao:
            EtapaAprovacaoSemRegra();
            break;
    }
}

/*
 * Declaração das Funções da Etapa Um (Dados do Termo)
 */

function EtapaDadosTermoQuitacaoAprovada() {
    $("#" + _etapaTermoQuitacao.EtapaDadosTermoQuitacao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaTermoQuitacao.EtapaDadosTermoQuitacao.idTab + " .step").attr("class", "step green");
}

function EtapaDadosTermoQuitacaoLiberada() {
    $("#" + _etapaTermoQuitacao.EtapaDadosTermoQuitacao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaTermoQuitacao.EtapaDadosTermoQuitacao.idTab + " .step").attr("class", "step yellow");

    EtapaDadosAceiteTransportadorDesabilitada();
}

/*
 * Declaração das Funções da Etapa Dois (Aceite do Transportador)
 */

function EtapaDadosAceiteTransportadorAguardando() {
    $("#" + _etapaTermoQuitacao.EtapaDadosAceiteTransportador.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaTermoQuitacao.EtapaDadosAceiteTransportador.idTab + " .step").attr("class", "step yellow");

    _etapaDadosAceiteTransportadorAtiva = true;

    EtapaDadosTermoQuitacaoAprovada();
    EtapaAprovacaoDesabilitada();
}

function EtapaDadosAceiteTransportadorAprovada() {
    $("#" + _etapaTermoQuitacao.EtapaDadosAceiteTransportador.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaTermoQuitacao.EtapaDadosAceiteTransportador.idTab + " .step").attr("class", "step green");

    _etapaDadosAceiteTransportadorAtiva = true;

    EtapaDadosTermoQuitacaoAprovada();
}

function EtapaDadosAceiteTransportadorDesabilitada() {
    $("#" + _etapaTermoQuitacao.EtapaDadosAceiteTransportador.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaTermoQuitacao.EtapaDadosAceiteTransportador.idTab + " .step").attr("class", "step");

    _etapaDadosAceiteTransportadorAtiva = false;

    EtapaAprovacaoDesabilitada();
}

function EtapaDadosAceiteTransportadorRejeitada() {
    $("#" + _etapaTermoQuitacao.EtapaDadosAceiteTransportador.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaTermoQuitacao.EtapaDadosAceiteTransportador.idTab + " .step").attr("class", "step red");

    _etapaDadosAceiteTransportadorAtiva = true;

    EtapaDadosTermoQuitacaoAprovada();
}

/*
 * Declaração das Funções da Etapa Três (Aprovação)
 */

function EtapaAprovacaoAguardando() {
    $("#" + _etapaTermoQuitacao.EtapaAprovacao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaTermoQuitacao.EtapaAprovacao.idTab + " .step").attr("class", "step yellow");

    _etapaAprovacaoAtiva = true;

    EtapaDadosAceiteTransportadorAprovada();
}

function EtapaAprovacaoAprovada() {
    $("#" + _etapaTermoQuitacao.EtapaAprovacao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaTermoQuitacao.EtapaAprovacao.idTab + " .step").attr("class", "step green");

    _etapaAprovacaoAtiva = true;

    EtapaDadosAceiteTransportadorAprovada();
}

function EtapaAprovacaoDesabilitada() {
    $("#" + _etapaTermoQuitacao.EtapaAprovacao.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaTermoQuitacao.EtapaAprovacao.idTab + " .step").attr("class", "step");

    _etapaAprovacaoAtiva = false;
}

function EtapaAprovacaoRejeitada() {
    $("#" + _etapaTermoQuitacao.EtapaAprovacao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaTermoQuitacao.EtapaAprovacao.idTab + " .step").attr("class", "step red");

    _etapaAprovacaoAtiva = true;

    EtapaDadosAceiteTransportadorAprovada();
}

function EtapaAprovacaoSemRegra() {
    $("#" + _etapaTermoQuitacao.EtapaAprovacao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaTermoQuitacao.EtapaAprovacao.idTab + " .step").attr("class", "step red");

    _etapaAprovacaoAtiva = true;

    EtapaDadosAceiteTransportadorAprovada();
    EtapaSemRegra();
}
