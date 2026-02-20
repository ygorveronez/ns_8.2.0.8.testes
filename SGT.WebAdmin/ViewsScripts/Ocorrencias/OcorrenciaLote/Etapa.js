/// <reference path="../../Enumeradores/EnumSituacaoOcorrenciaLote.js" />
/// <reference path="OcorrenciaLote.js" />
/// <reference path="Ocorrencia.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _etapaOcorrenciaLote;

var EtapaOcorrenciaLote = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("50%") });

    this.Etapa1 = PropertyEntity({
        text: "Dados", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("É onde preenche os dados para geração."),
        tooltipTitle: ko.observable("Dados")
    });
    this.Etapa2 = PropertyEntity({
        text: "Ocorrências", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("É onde visualiza as ocorrências geradas."),
        tooltipTitle: ko.observable("Ocorrências")
    });
};

//*******EVENTOS*******

function LoadEtapaOcorrenciaLote() {
    _etapaOcorrenciaLote = new EtapaOcorrenciaLote();
    KoBindings(_etapaOcorrenciaLote, "knockoutEtapaOcorrenciaLote");

    Etapa1Liberada();
}

function SetarEtapaInicioOcorrenciaLote() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapaOcorrenciaLote.Etapa1.idTab).click();
    $("#" + _etapaOcorrenciaLote.Etapa1.idTab).tab("show")
}

function SetarEtapaOcorrenciaLote() {
    var situacao = _ocorrenciaLote.Situacao.val();

    if (situacao === EnumSituacaoOcorrenciaLote.EmGeracao)
        Etapa2Aguardando();
    else if (situacao === EnumSituacaoOcorrenciaLote.Finalizado)
        Etapa2Aprovada();
    else if (situacao === EnumSituacaoOcorrenciaLote.FalhaNaGeracao)
        Etapa2Reprovada();
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaOcorrenciaLote.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOcorrenciaLote.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaOcorrenciaLote.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOcorrenciaLote.Etapa1.idTab + " .step").attr("class", "step green");
}

function Etapa1Desabilitada() {
    $("#" + _etapaOcorrenciaLote.Etapa1.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaOcorrenciaLote.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    _etapaOcorrenciaLote.Etapa2.eventClick = function () { };
    $("#" + _etapaOcorrenciaLote.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaOcorrenciaLote.Etapa2.idTab + " .step").attr("class", "step");
}

function Etapa2Liberada() {
    _etapaOcorrenciaLote.Etapa2.eventClick = EtapaOcorrenciaLoteOcorrenciaClick;
    $("#" + _etapaOcorrenciaLote.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOcorrenciaLote.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
}

function Etapa2Aguardando() {
    _etapaOcorrenciaLote.Etapa2.eventClick = EtapaOcorrenciaLoteOcorrenciaClick;
    $("#" + _etapaOcorrenciaLote.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOcorrenciaLote.Etapa2.idTab + " .step").attr("class", "step yellow");
    $("#" + _etapaOcorrenciaLote.Etapa2.idTab).click();
    $("#" + _etapaOcorrenciaLote.Etapa2.idTab).tab("show")
    Etapa1Aprovada();
}

function Etapa2Aprovada() {
    _etapaOcorrenciaLote.Etapa2.eventClick = EtapaOcorrenciaLoteOcorrenciaClick;
    $("#" + _etapaOcorrenciaLote.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOcorrenciaLote.Etapa2.idTab + " .step").attr("class", "step green");
    $("#" + _etapaOcorrenciaLote.Etapa2.idTab).click();
    $("#" + _etapaOcorrenciaLote.Etapa2.idTab).tab("show")
    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    _etapaOcorrenciaLote.Etapa2.eventClick = EtapaOcorrenciaLoteOcorrenciaClick;
    $("#" + _etapaOcorrenciaLote.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOcorrenciaLote.Etapa2.idTab + " .step").attr("class", "step red");
    $("#" + _etapaOcorrenciaLote.Etapa2.idTab).click();
    $("#" + _etapaOcorrenciaLote.Etapa2.idTab).tab("show")
    Etapa1Aprovada();
}