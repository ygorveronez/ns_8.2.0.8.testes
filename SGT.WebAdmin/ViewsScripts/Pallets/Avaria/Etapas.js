/// <reference path="avaria.js" />
/// <reference path="../../Enumeradores/EnumAbaAvariaPallet.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAvariaPallet.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _etapaAvariaPallet;
var _etapaAprovacaoAtiva = false;

/*
 * Declaração das Classes
 */

var EtapaAvariaPallet = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("50%") });

    this.EtapaAprovacao = PropertyEntity({
        text: "Aprovação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaAprovacaoClick,
        step: ko.observable(2),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("Aprovação")
    });

    this.EtapaDadosAvaria = PropertyEntity({
        text: "Dados da Avaria", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaDadosAvariaClick,
        step: ko.observable(1),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("Dados da Avaria")
    });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadEtapaAvariaPallet() {
    _etapaAvariaPallet = new EtapaAvariaPallet();
    KoBindings(_etapaAvariaPallet, "knockoutEtapaAvariaPallet");

    EtapaDadosAvariaLiberada();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function etapaAprovacaoClick() {
    if (_etapaAprovacaoAtiva) {
        _abaAtiva = EnumAbaAvariaPallet.Aprovacao;

        controlarBotoesHabilitados();
    }
}

function etapaDadosAvariaClick() {
    _abaAtiva = EnumAbaAvariaPallet.DadosAvaria;

    controlarBotoesHabilitados();
}

/*
 * Declaração das Funções Públicas
 */

function EtapaSemRegra() {
    exibirMensagem(tipoMensagem.aviso, "Regras da etapa", "Nenhuma regra encontrada. A solicitação permanecerá aguardando autorização.");

    _CRUDAvariaPallet.ReprocessarRegras.visible(true);
}

function setarEtapaInicial() {
    EtapaDadosAvariaLiberada();

    $("#" + _etapaAvariaPallet.EtapaDadosAvaria.idTab).click();
    $("#" + _etapaAvariaPallet.EtapaDadosAvaria.idTab).tab("show");
}

function setarEtapas() {
    $("#" + _etapaAvariaPallet.EtapaDadosAvaria.idTab).click();
    $("#" + _etapaAvariaPallet.EtapaDadosAvaria.idTab).tab("show");

    switch (_avariaPallet.Situacao.val()) {
        case EnumSituacaoAvariaPallet.AguardandoAprovacao:
            EtapaAprovacaoAguardando();
            break;

        case EnumSituacaoAvariaPallet.AprovacaoRejeitada:
            EtapaAprovacaoRejeitada();
            break;

        case EnumSituacaoAvariaPallet.Finalizada:
            EtapaAprovacaoAprovada();
            break;

        case EnumSituacaoAvariaPallet.Todas:
            EtapaDadosAvariaLiberada();
            break;

        case EnumSituacaoAvariaPallet.SemRegraAprovacao:
            EtapaAprovacaoSemRegra();
            break;
    }
}

/*
 * Declaração das Funções da Etapa Um (Dados da Avaria)
 */

function EtapaDadosAvariaAprovada() {
    $("#" + _etapaAvariaPallet.EtapaDadosAvaria.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAvariaPallet.EtapaDadosAvaria.idTab + " .step").attr("class", "step green");
}

function EtapaDadosAvariaLiberada() {
    $("#" + _etapaAvariaPallet.EtapaDadosAvaria.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAvariaPallet.EtapaDadosAvaria.idTab + " .step").attr("class", "step yellow");

    EtapaAprovacaoDesabilitada();
}

/*
 * Declaração das Funções da Etapa Dois (Aprovação)
 */

function EtapaAprovacaoAguardando() {
    $("#" + _etapaAvariaPallet.EtapaAprovacao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAvariaPallet.EtapaAprovacao.idTab + " .step").attr("class", "step yellow");

    _etapaAprovacaoAtiva = true;

    EtapaDadosAvariaAprovada();
}

function EtapaAprovacaoAprovada() {
    $("#" + _etapaAvariaPallet.EtapaAprovacao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAvariaPallet.EtapaAprovacao.idTab + " .step").attr("class", "step green");

    _etapaAprovacaoAtiva = true;

    EtapaDadosAvariaAprovada();
}

function EtapaAprovacaoDesabilitada() {
    $("#" + _etapaAvariaPallet.EtapaAprovacao.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaAvariaPallet.EtapaAprovacao.idTab + " .step").attr("class", "step");

    _etapaAprovacaoAtiva = false;
}

function EtapaAprovacaoRejeitada() {
    $("#" + _etapaAvariaPallet.EtapaAprovacao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAvariaPallet.EtapaAprovacao.idTab + " .step").attr("class", "step red");

    _etapaAprovacaoAtiva = true;

    EtapaDadosAvariaAprovada();
}

function EtapaAprovacaoSemRegra() {
    $("#" + _etapaAvariaPallet.EtapaAprovacao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAvariaPallet.EtapaAprovacao.idTab + " .step").attr("class", "step red");

    _etapaAprovacaoAtiva = true;

    EtapaDadosAvariaAprovada();
    EtapaSemRegra();
}
