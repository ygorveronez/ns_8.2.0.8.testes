/// <reference path="RFIConvite.js" />
/// <reference path="../../Enumeradores/EnumSituacaoRFIConvite.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _etapaRFIConvite

var EtapaRFIConvite = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("20%") });

    this.Etapa1 = PropertyEntity({
        text: "RFI Convite", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("É onde se gera o RFI Convite."),
        tooltipTitle: ko.observable("RFI Convite")
    });

    this.Etapa2 = PropertyEntity({
        text: "Aprovação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("É a etapa de aprovação do RFI Convite."),
        tooltipTitle: ko.observable("Aprovação")
    });
};

//*******EVENTOS*******

function LoadEtapaRFIConvite() {
    _etapaRFIConvite = new EtapaRFIConvite();
    KoBindings(_etapaRFIConvite, "knockoutRFIConvite");
    Etapa1Liberada();
}

function SetarEtapaInicioRFIConvite() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapaRFIConvite.Etapa1.idTab).click();
    $("#" + _etapaRFIConvite.Etapa1.idTab).tab("show")
}

function SetarEtapaRFIConvite() {
    const situacaoRFIConvite = _RFIConvite.Situacao.val();
    if (situacaoRFIConvite === EnumSituacaoRFIConvite.Finalizada
        || situacaoRFIConvite === EnumSituacaoRFIConvite.Fechamento
    )
        Etapa2Aprovada();
    else if (situacaoRFIConvite === EnumSituacaoRFIConvite.Checklist)
        Etapa2Aguardando();
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaRFIConvite.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaRFIConvite.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaRFIConvite.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaRFIConvite.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    _etapaRFIConvite.Etapa2.eventClick = function () { };
    $("#" + _etapaRFIConvite.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaRFIConvite.Etapa2.idTab + " .step").attr("class", "step");
}

function Etapa2Liberada() {
    _etapaRFIConvite.Etapa2.eventClick = CarregarAprovacao;
    $("#" + _etapaRFIConvite.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaRFIConvite.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
}

function Etapa2Aguardando() {
    _etapaRFIConvite.Etapa2.eventClick = CarregarAprovacao;
    $("#" + _etapaRFIConvite.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaRFIConvite.Etapa2.idTab + " .step").attr("class", "step yellow");
    $("#" + _etapaRFIConvite.Etapa2.idTab).click();
    $("#" + _etapaRFIConvite.Etapa2.idTab).tab("show");
    Etapa1Aprovada();
}

function Etapa2Aprovada() {
    _etapaRFIConvite.Etapa2.eventClick = CarregarAprovacao;
    $("#" + _etapaRFIConvite.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaRFIConvite.Etapa2.idTab + " .step").attr("class", "step green");
    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    _etapaRFIConvite.Etapa2.eventClick = CarregarAprovacao;
    $("#" + _etapaRFIConvite.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaRFIConvite.Etapa2.idTab + " .step").attr("class", "step red");
    $("#" + _etapaRFIConvite.Etapa2.idTab).click();
    $("#" + _etapaRFIConvite.Etapa2.idTab).tab("show");
    Etapa1Aprovada();
}