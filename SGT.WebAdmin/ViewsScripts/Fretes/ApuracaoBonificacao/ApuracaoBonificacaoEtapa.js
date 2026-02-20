/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumSituacaoFechamentoPontuacao.js" />
/// <reference path="ApuracaoBonificacao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _apuracaoBonificacaoEtapa;

/*
 * Declaração das Classes
 */

var ApuracaoBonificacaoEtapa = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("50%") });

    this.EtapaDadosApuracaoBonificacao = PropertyEntity({
        text: "Dados do Fechamento", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("Dados do Fechamento")
    });

    this.EtapaApuracoes = PropertyEntity({
        text: "Apurações", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("Apurações")
    });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadApuracaoBonificacaoEtapa() {
    _apuracaoBonificacaoEtapa = new ApuracaoBonificacaoEtapa();
    KoBindings(_apuracaoBonificacaoEtapa, "knockoutApuracaoBonificacaoEtapa");

    EtapaDadosApuracaoBonificacaoLiberada();
}

/*
 * Declaração das Funções Públicas
 */

function setarEtapaInicial() {
    EtapaDadosApuracaoBonificacaoLiberada();

    $("#" + _apuracaoBonificacaoEtapa.EtapaDadosApuracaoBonificacao.idTab).click();
}

function setarEtapas() {
    $("#" + _apuracaoBonificacaoEtapa.EtapaDadosApuracaoBonificacao.idTab).click();

    switch (_apuracaoBonificacao.Situacao.val()) {
        case EnumSituacaoApuracaoBonificacao.AguardandoGeracaoOcorrencia:
            EtapaApuracoesAguardando();
            break;

        case EnumSituacaoApuracaoBonificacao.Cancelado:
            EtapaApuracoesCancelada();
            break;

        case EnumSituacaoApuracaoBonificacao.Finalizado:
            EtapaApuracoesAprovada();
            break;

        case EnumSituacaoApuracaoBonificacao.Todas:
            EtapaDadosApuracaoBonificacaoLiberada();
            break;
    }
}

/*
 * Declaração das Funções Privadas da Etapa Um (Dados do Fechamento PGT)
 */

function EtapaDadosApuracaoBonificacaoAprovada() {
    $("#" + _apuracaoBonificacaoEtapa.EtapaDadosApuracaoBonificacao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _apuracaoBonificacaoEtapa.EtapaDadosApuracaoBonificacao.idTab + " .step").attr("class", "step green");
}

function EtapaDadosApuracaoBonificacaoLiberada() {
    $("#" + _apuracaoBonificacaoEtapa.EtapaDadosApuracaoBonificacao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _apuracaoBonificacaoEtapa.EtapaDadosApuracaoBonificacao.idTab + " .step").attr("class", "step yellow");

    EtapaApuracoesDesabilitada();
}

/*
 * Declaração das Funções Privadas da Etapa Dois (Apurações)
 */

function EtapaApuracoesAguardando() {
    $("#" + _apuracaoBonificacaoEtapa.EtapaApuracoes.idTab).attr("data-bs-toggle", "tab");
    $("#" + _apuracaoBonificacaoEtapa.EtapaApuracoes.idTab + " .step").attr("class", "step yellow");

    EtapaDadosApuracaoBonificacaoAprovada();
}

function EtapaApuracoesAprovada() {
    $("#" + _apuracaoBonificacaoEtapa.EtapaApuracoes.idTab).attr("data-bs-toggle", "tab");
    $("#" + _apuracaoBonificacaoEtapa.EtapaApuracoes.idTab + " .step").attr("class", "step green");

    EtapaDadosApuracaoBonificacaoAprovada();
}

function EtapaApuracoesCancelada() {
    $("#" + _apuracaoBonificacaoEtapa.EtapaApuracoes.idTab).attr("data-bs-toggle", "tab");
    $("#" + _apuracaoBonificacaoEtapa.EtapaApuracoes.idTab + " .step").attr("class", "step red");

    EtapaDadosApuracaoBonificacaoAprovada();
}

function EtapaApuracoesDesabilitada() {
    $("#" + _apuracaoBonificacaoEtapa.EtapaApuracoes.idTab).removeAttr("data-bs-toggle");
    $("#" + _apuracaoBonificacaoEtapa.EtapaApuracoes.idTab + " .step").attr("class", "step");
}
