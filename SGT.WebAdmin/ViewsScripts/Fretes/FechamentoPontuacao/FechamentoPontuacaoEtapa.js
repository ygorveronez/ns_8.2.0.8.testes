/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumSituacaoFechamentoPontuacao.js" />
/// <reference path="FechamentoPontuacao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _fechamentoPontuacaoEtapa;

/*
 * Declaração das Classes
 */

var FechamentoPontuacaoEtapa = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("50%") });

    this.EtapaPontuacaoTransportadores = PropertyEntity({
        text: "Pontuações dos Transportadores", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("Pontuações dos Transportadores")
    });

    this.EtapaDadosFechamentoPontuacao = PropertyEntity({
        text: "Dados do Fechamento", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("Dados do Fechamento de Pontuação")
    });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadFechamentoPontuacaoEtapa() {
    _fechamentoPontuacaoEtapa = new FechamentoPontuacaoEtapa();
    KoBindings(_fechamentoPontuacaoEtapa, "knockoutFechamentoPontuacaoEtapa");

    etapaDadosFechamentoPontuacaoLiberada();
}

/*
 * Declaração das Funções Públicas
 */

function setarEtapaInicial() {
    etapaDadosFechamentoPontuacaoLiberada();

    $("#" + _fechamentoPontuacaoEtapa.EtapaDadosFechamentoPontuacao.idTab).click();
}

function setarEtapas() {
    $("#" + _fechamentoPontuacaoEtapa.EtapaDadosFechamentoPontuacao.idTab).click();

    switch (_fechamentoPontuacao.Situacao.val()) {
        case EnumSituacaoFechamentoPontuacao.AguardandoFinalizacao:
            etapaPontuacaoTransportadoresAguardando();
            break;

        case EnumSituacaoFechamentoPontuacao.Cancelado:
            etapaPontuacaoTransportadoresCancelada();
            break;

        case EnumSituacaoFechamentoPontuacao.Finalizado:
            etapaPontuacaoTransportadoresAprovada();
            break;

        case EnumSituacaoFechamentoPontuacao.Todas:
            etapaDadosFechamentoPontuacaoLiberada();
            break;
    }
}

/*
 * Declaração das Funções Privadas da Etapa Um (Dados do Fechamento de Pontuação)
 */

function etapaDadosFechamentoPontuacaoAprovada() {
    $("#" + _fechamentoPontuacaoEtapa.EtapaDadosFechamentoPontuacao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _fechamentoPontuacaoEtapa.EtapaDadosFechamentoPontuacao.idTab + " .step").attr("class", "step green");
}

function etapaDadosFechamentoPontuacaoLiberada() {
    $("#" + _fechamentoPontuacaoEtapa.EtapaDadosFechamentoPontuacao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _fechamentoPontuacaoEtapa.EtapaDadosFechamentoPontuacao.idTab + " .step").attr("class", "step yellow");

    etapaPontuacaoTransportadoresDesabilitada();
}

/*
 * Declaração das Funções Privadas da Etapa Dois (Pontuações dos Transportadores)
 */

function etapaPontuacaoTransportadoresAguardando() {
    $("#" + _fechamentoPontuacaoEtapa.EtapaPontuacaoTransportadores.idTab).attr("data-bs-toggle", "tab");
    $("#" + _fechamentoPontuacaoEtapa.EtapaPontuacaoTransportadores.idTab + " .step").attr("class", "step yellow");

    etapaDadosFechamentoPontuacaoAprovada();
}

function etapaPontuacaoTransportadoresAprovada() {
    $("#" + _fechamentoPontuacaoEtapa.EtapaPontuacaoTransportadores.idTab).attr("data-bs-toggle", "tab");
    $("#" + _fechamentoPontuacaoEtapa.EtapaPontuacaoTransportadores.idTab + " .step").attr("class", "step green");

    etapaDadosFechamentoPontuacaoAprovada();
}

function etapaPontuacaoTransportadoresCancelada() {
    $("#" + _fechamentoPontuacaoEtapa.EtapaPontuacaoTransportadores.idTab).attr("data-bs-toggle", "tab");
    $("#" + _fechamentoPontuacaoEtapa.EtapaPontuacaoTransportadores.idTab + " .step").attr("class", "step red");

    etapaDadosFechamentoPontuacaoAprovada();
}

function etapaPontuacaoTransportadoresDesabilitada() {
    $("#" + _fechamentoPontuacaoEtapa.EtapaPontuacaoTransportadores.idTab).removeAttr("data-bs-toggle");
    $("#" + _fechamentoPontuacaoEtapa.EtapaPontuacaoTransportadores.idTab + " .step").attr("class", "step");
}
