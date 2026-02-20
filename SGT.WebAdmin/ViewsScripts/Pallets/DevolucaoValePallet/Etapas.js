/// <reference path="DevolucaoValePallet.js" />
/// <reference path="../../Enumeradores/EnumAbaDevolucaoValePallet.js" />
/// <reference path="../../Enumeradores/EnumSituacaoDevolucaoValePallet.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _etapaDevolucaoValePallet;
var _etapaAprovacaoAtiva = false;

/*
 * Declaração das Classes
 */

var EtapaDevolucaoValePallet = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("50%") });

    this.EtapaAprovacao = PropertyEntity({
        text: "Aprovação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaAprovacaoClick,
        step: ko.observable(2),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("Aprovação")
    });

    this.EtapaDadosDevolucao = PropertyEntity({
        text: "Devolução", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaDadosDevolucaoClick,
        step: ko.observable(1),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("Devolução")
    });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadEtapaDevolucaoValePallet() {
    _etapaDevolucaoValePallet = new EtapaDevolucaoValePallet();
    KoBindings(_etapaDevolucaoValePallet, "knockoutEtapaDevolucaoValePallet");

    EtapaDadosDevolucaoLiberada();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function etapaAprovacaoClick() {
    if (_etapaAprovacaoAtiva) {
        _abaAtiva = EnumAbaDevolucaoValePallet.Aprovacao;

        controlarBotoesHabilitados();
    }
}

function etapaDadosDevolucaoClick() {
    _abaAtiva = EnumAbaDevolucaoValePallet.DadosDevolucao;

    controlarBotoesHabilitados();
}

/*
 * Declaração das Funções Públicas
 */

function EtapaSemRegra() {
    exibirMensagem(tipoMensagem.aviso, "Regras da etapa", "Nenhuma regra encontrada. A solicitação permanecerá aguardando autorização.");

    _CRUDDevolucaoValePallet.ReprocessarRegras.visible(true);
}

function setarEtapas() {
    $("#" + _etapaDevolucaoValePallet.EtapaDadosDevolucao.idTab).click();

    switch (_devolucaoValePallet.Situacao.val()) {
        case EnumSituacaoDevolucaoValePallet.AguardandoAprovacao:
            EtapaAprovacaoAguardando();
            break;

        case EnumSituacaoDevolucaoValePallet.AprovacaoRejeitada:
            EtapaAprovacaoRejeitada();
            break;

        case EnumSituacaoDevolucaoValePallet.Finalizada:
            EtapaAprovacaoAprovada();
            break;

        case EnumSituacaoDevolucaoValePallet.Todas:
            EtapaDadosDevolucaoLiberada();
            break;

        case EnumSituacaoDevolucaoValePallet.SemRegraAprovacao:
            EtapaAprovacaoSemRegra();
            break;
    }
}

/*
 * Declaração das Funções da Etapa Um (Solicitação)
 */

function EtapaDadosDevolucaoAprovada() {
    $("#" + _etapaDevolucaoValePallet.EtapaDadosDevolucao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaDevolucaoValePallet.EtapaDadosDevolucao.idTab + " .step").attr("class", "step green");
}

function EtapaDadosDevolucaoLiberada() {
    $("#" + _etapaDevolucaoValePallet.EtapaDadosDevolucao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaDevolucaoValePallet.EtapaDadosDevolucao.idTab + " .step").attr("class", "step yellow");

    EtapaAprovacaoDesabilitada();
}

/*
 * Declaração das Funções da Etapa Dois (Aprovação)
 */

function EtapaAprovacaoAguardando() {
    $("#" + _etapaDevolucaoValePallet.EtapaAprovacao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaDevolucaoValePallet.EtapaAprovacao.idTab + " .step").attr("class", "step yellow");

    _etapaAprovacaoAtiva = true;

    EtapaDadosDevolucaoAprovada();
}

function EtapaAprovacaoAprovada() {
    $("#" + _etapaDevolucaoValePallet.EtapaAprovacao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaDevolucaoValePallet.EtapaAprovacao.idTab + " .step").attr("class", "step green");

    _etapaAprovacaoAtiva = true;

    EtapaDadosDevolucaoAprovada();
}

function EtapaAprovacaoDesabilitada() {
    $("#" + _etapaDevolucaoValePallet.EtapaAprovacao.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaDevolucaoValePallet.EtapaAprovacao.idTab + " .step").attr("class", "step");

    _etapaAprovacaoAtiva = false;
}

function EtapaAprovacaoRejeitada() {
    $("#" + _etapaDevolucaoValePallet.EtapaAprovacao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaDevolucaoValePallet.EtapaAprovacao.idTab + " .step").attr("class", "step red");

    _etapaAprovacaoAtiva = true;

    EtapaDadosDevolucaoAprovada();
}

function EtapaAprovacaoSemRegra() {
    $("#" + _etapaDevolucaoValePallet.EtapaAprovacao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaDevolucaoValePallet.EtapaAprovacao.idTab + " .step").attr("class", "step red");

    _etapaAprovacaoAtiva = true;

    EtapaDadosDevolucaoAprovada();
    EtapaSemRegra();
}