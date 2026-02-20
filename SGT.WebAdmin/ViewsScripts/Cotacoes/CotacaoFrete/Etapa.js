/// <reference path="CotacaoFrete.js" />
/// <reference path="Transportador.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _etapaCotacaoFrete;

var EtapaCotacaoFrete = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("50%") });

    this.Etapa1 = PropertyEntity({
        text: "Cotação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("É onde se inicia o cadastro."),
        tooltipTitle: ko.observable("Cotação")
    });
    this.Etapa2 = PropertyEntity({
        text: "Escolha de cotação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("É a etapa de escolha."),
        tooltipTitle: ko.observable("Escolha de cotação")
    });
};

//*******EVENTOS*******

function LoadEtapaCotacaoFrete() {
    _etapaCotacaoFrete = new EtapaCotacaoFrete();
    KoBindings(_etapaCotacaoFrete, "knockoutEtapaCotacaoFrete");

    Etapa1Liberada();
}

function SetarEtapaInicioCotacaoFrete() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapaCotacaoFrete.Etapa1.idTab).click();
    $("#" + _etapaCotacaoFrete.Etapa1.idTab).tab("show");
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaCotacaoFrete.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCotacaoFrete.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaCotacaoFrete.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCotacaoFrete.Etapa1.idTab + " .step").attr("class", "step green");
}

function Etapa1Desabilitada() {
    $("#" + _etapaCotacaoFrete.Etapa1.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaCotacaoFrete.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    $("#" + _etapaCotacaoFrete.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaCotacaoFrete.Etapa2.idTab + " .step").attr("class", "step");
}

function Etapa2Liberada() {
    $("#" + _etapaCotacaoFrete.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCotacaoFrete.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
}

function Etapa2Aguardando() {
    $("#" + _etapaCotacaoFrete.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCotacaoFrete.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Desabilitada();
}

function Etapa2Aprovada() {
    $("#" + _etapaCotacaoFrete.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCotacaoFrete.Etapa2.idTab + " .step").attr("class", "step green");
    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    $("#" + _etapaCotacaoFrete.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCotacaoFrete.Etapa2.idTab + " .step").attr("class", "step red");
    Etapa1Aprovada();
}