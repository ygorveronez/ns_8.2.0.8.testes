/// <reference path="BiddingConvite.js" />
/// <reference path="Aprovacao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoBiddingConvite.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _etapaBiddingConvite

var EtapaBiddingConvite = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("20%") });

    this.Etapa1 = PropertyEntity({
        text: "Bidding Convite", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("É onde se gera o Bidding Convite."),
        tooltipTitle: ko.observable("Bidding Convite")
    });

    this.Etapa2 = PropertyEntity({
        text: "Aprovação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("É a etapa de aprovação do Bidding Convite."),
        tooltipTitle: ko.observable("Aprovação")
    });

};

//*******EVENTOS*******

function LoadEtapaBiddingConvite() {
    _etapaBiddingConvite = new EtapaBiddingConvite();
    KoBindings(_etapaBiddingConvite, "knockoutBiddingConvite");
    Etapa1Liberada();
}

function SetarEtapaInicioBiddingConvite() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapaBiddingConvite.Etapa1.idTab).click();
    $("#" + _etapaBiddingConvite.Etapa1.idTab).tab("show")
}

function SetarEtapaBiddingConvite() {
    const situacaoBiddingConvite = _biddingConvite.SituacaoAprovacao.val();
    if (situacaoBiddingConvite === EnumSituacaoBiddingConvite.Finalizada
        || situacaoBiddingConvite === EnumSituacaoBiddingConvite.Aguardando
        || situacaoBiddingConvite === EnumSituacaoBiddingConvite.Checklist
        || situacaoBiddingConvite === EnumSituacaoBiddingConvite.Fechamento
        || situacaoBiddingConvite === EnumSituacaoBiddingConvite.Ofertas
    )
        Etapa2Aprovada();
    else if (situacaoBiddingConvite === EnumSituacaoBiddingConvite.AguardandoAprovacao || situacaoBiddingConvite === EnumSituacaoBiddingConvite.SemRegra)
        Etapa2Aguardando();
    else if (situacaoBiddingConvite === EnumSituacaoBiddingConvite.AprovacaoRejeitada)
        Etapa2Reprovada();
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaBiddingConvite.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaBiddingConvite.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaBiddingConvite.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaBiddingConvite.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    _etapaBiddingConvite.Etapa2.eventClick = function () { };
    $("#" + _etapaBiddingConvite.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaBiddingConvite.Etapa2.idTab + " .step").attr("class", "step");
}

function Etapa2Liberada() {
    _etapaBiddingConvite.Etapa2.eventClick = CarregarAprovacaoBiddingConvite;
    $("#" + _etapaBiddingConvite.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaBiddingConvite.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
}

function Etapa2Aguardando() {
    _etapaBiddingConvite.Etapa2.eventClick = CarregarAprovacaoBiddingConvite;
    $("#" + _etapaBiddingConvite.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaBiddingConvite.Etapa2.idTab + " .step").attr("class", "step yellow");
    $("#" + _etapaBiddingConvite.Etapa2.idTab).click();
    $("#" + _etapaBiddingConvite.Etapa2.idTab).tab("show");
    Etapa1Aprovada();
}

function Etapa2Aprovada() {
    _etapaBiddingConvite.Etapa2.eventClick = CarregarAprovacaoBiddingConvite;
    $("#" + _etapaBiddingConvite.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaBiddingConvite.Etapa2.idTab + " .step").attr("class", "step green");
    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    _etapaBiddingConvite.Etapa2.eventClick = CarregarAprovacaoBiddingConvite;
    $("#" + _etapaBiddingConvite.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaBiddingConvite.Etapa2.idTab + " .step").attr("class", "step red");
    $("#" + _etapaBiddingConvite.Etapa2.idTab).click();
    $("#" + _etapaBiddingConvite.Etapa2.idTab).tab("show");
    Etapa1Aprovada();
}