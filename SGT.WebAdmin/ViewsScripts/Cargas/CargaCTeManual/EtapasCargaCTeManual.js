/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="CargaCTeManual.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _etapaCargaCTeManual;

var EtapaCargaCTeManual = function () {
    var _this = this;

    this.Etapa1 = PropertyEntity({
        text: "Emissão", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("")
    });

    this.Etapa2 = PropertyEntity({
        text: "Integração", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("")
    });

    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("") });
    this.ExibirEtapas = PropertyEntity({ visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadEtapaCargaCTeManual() {
    _etapaCargaCTeManual = new EtapaCargaCTeManual();
    KoBindings(_etapaCargaCTeManual, "knockoutEtapaCargaCTeManual");
    Etapa1Liberada();
    $("#" + _etapaCargaCTeManual.Etapa1.idTab).click();
}

function AjustarEtapasCargaCTeManual() {
    _etapaCargaCTeManual.TamanhoEtapa.val("50%");
    _etapaCargaCTeManual.Etapa1.step(1);
    _etapaCargaCTeManual.Etapa2.step(2);
}

function setarEtapaInicioCargaCTeManual() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapaCargaCTeManual.Etapa1.idTab).click();
    $("#" + _etapaCargaCTeManual.Etapa1.idTab).tab("show");
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
    AjustarEtapasCargaCTeManual();
    _etapaCargaCTeManual.ExibirEtapas.visible(false);
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaCargaCTeManual.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCargaCTeManual.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaCargaCTeManual.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCargaCTeManual.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    $("#" + _etapaCargaCTeManual.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaCargaCTeManual.Etapa2.idTab + " .step").attr("class", "step");
}

function Etapa2Liberada() {
    $("#" + _etapaCargaCTeManual.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCargaCTeManual.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
}

function Etapa2Aprovada() {
    $("#" + _etapaCargaCTeManual.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCargaCTeManual.Etapa2.idTab + " .step").attr("class", "step green");
    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    $("#" + _etapaCargaCTeManual.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCargaCTeManual.Etapa2.idTab + " .step").attr("class", "step red");
    Etapa1Aprovada();
}