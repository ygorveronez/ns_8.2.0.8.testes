/// <reference path="AtendimentoCRM.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _etapaAtendimentoCRM;

var EtapaAtendimentoCRM = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("50%") });

    this.Etapa1 = PropertyEntity({
        text: "Abertura", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("É onde preenche os dados para iniciar o atendimento."),
        tooltipTitle: ko.observable("Abertura")
    });
    this.Etapa2 = PropertyEntity({
        text: "Análise", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("É onde visualiza os atendimentos."),
        tooltipTitle: ko.observable("Análise")
    });
    this.Etapa3 = PropertyEntity({
        text: "Finalizado", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable("É onde visualiza as atendimentos."),
        tooltipTitle: ko.observable("Finalizado")
    });
};

function LoadEtapaAtendimentoCRM() {    
    _etapaAtendimentoCRM = new EtapaAtendimentoCRM();
    KoBindings(_etapaAtendimentoCRM, "knockoutEtapaAtendimentoCRM");    
    Etapa1Liberada();
}

//*******Etapa 1*******
function Etapa1Liberada() {
    $("#" + _etapaAtendimentoCRM.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaAtendimentoCRM.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
    Etapa3Desabilitada();
}


//*******Etapa 2*******
function Etapa2Desabilitada() {
    _etapaAtendimentoCRM.Etapa2.eventClick = function () { };
    $("#" + _etapaAtendimentoCRM.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaAtendimentoCRM.Etapa2.idTab + " .step").attr("class", "step");
}


//*******Etapa 3*******

function Etapa3Desabilitada() {
    _etapaAtendimentoCRM.Etapa2.eventClick = function () { };
    $("#" + _etapaAtendimentoCRM.Etapa3.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaAtendimentoCRM.Etapa3.idTab + " .step").attr("class", "step");
}
