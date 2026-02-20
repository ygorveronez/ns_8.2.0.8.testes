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
/// <reference path="../../Enumeradores/EnumSituacaoCIOT.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _etapaCIOT;

var EtapaCIOT = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("33.33%") });

    this.Etapa1 = PropertyEntity({
        text: "Abertura", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("É onde se abre um CIOT."),
        tooltipTitle: ko.observable("Abertura")
    });
    this.Etapa2 = PropertyEntity({
        text: "Documentos", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("É a etapa de dos Documentos do CIOT."),
        tooltipTitle: ko.observable("Documentos")
    });
    this.Etapa3 = PropertyEntity({
        text: "Encerramento", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable("É a etapa de encerramento do CIOT."),
        tooltipTitle: ko.observable("Encerramento")
    });
}

//*******EVENTOS*******

function LoadEtapa() {
    _etapaCIOT = new EtapaCIOT();
    KoBindings(_etapaCIOT, "knockoutEtapaCIOT");
    Etapa1Liberada();
}

function SetarEtapaInicioCIOT() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapaCIOT.Etapa1.idTab).click();
    $("#" + _etapaCIOT.Etapa1.idTab).tab("show");
}

function SetarEtapaCIOT() {
    var situacaoCIOT = _CIOT.Situacao.val();

    if (situacaoCIOT == EnumSituacaoCIOT.Aberto || situacaoCIOT == EnumSituacaoCIOT.Cancelado)
        Etapa2Aguardando();
    else if (situacaoCIOT == EnumSituacaoCIOT.Encerrado)
        Etapa3Aprovada();
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
    Etapa3Desabilitada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaCIOT.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCIOT.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaCIOT.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCIOT.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    _etapaCIOT.Etapa2.eventClick = function () { };
    $("#" + _etapaCIOT.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaCIOT.Etapa2.idTab + " .step").attr("class", "step");
    Etapa3Desabilitada()
}

function Etapa2Liberada() {
    _etapaCIOT.Etapa2.eventClick = ConsultarCTesCIOT;
    $("#" + _etapaCIOT.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCIOT.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
}

function Etapa2Aguardando() {
    _etapaCIOT.Etapa2.eventClick = ConsultarCTesCIOT;
    $("#" + _etapaCIOT.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCIOT.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
}

function Etapa2Aprovada() {
    _etapaCIOT.Etapa2.eventClick = ConsultarCTesCIOT;
    $("#" + _etapaCIOT.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCIOT.Etapa2.idTab + " .step").attr("class", "step green");
    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    _etapaCIOT.Etapa2.eventClick = ConsultarCTesCIOT;
    $("#" + _etapaCIOT.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCIOT.Etapa2.idTab + " .step").attr("class", "step red");
    Etapa1Aprovada();
}


//*******Etapa 3*******

function Etapa3Desabilitada() {
    _etapaCIOT.Etapa3.eventClick = function () { };
    $("#" + _etapaCIOT.Etapa3.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaCIOT.Etapa3.idTab + " .step").attr("class", "step");
}

function Etapa3Liberada() {
    _etapaCIOT.Etapa3.eventClick = function () { };
    $("#" + _etapaCIOT.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCIOT.Etapa3.idTab + " .step").attr("class", "step yellow");
    Etapa2Aprovada();
}

function Etapa3Aguardando() {
    _etapaCIOT.Etapa3.eventClick = function () { };
    $("#" + _etapaCIOT.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCIOT.Etapa3.idTab + " .step").attr("class", "step yellow");
    Etapa2Aprovada();
}

function Etapa3Aprovada() {
    _etapaCIOT.Etapa3.eventClick = function () { };
    $("#" + _etapaCIOT.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCIOT.Etapa3.idTab + " .step").attr("class", "step green");
    Etapa2Aprovada();
}

function Etapa3Reprovada() {
    _etapaCIOT.Etapa3.eventClick = function () { };
    $("#" + _etapaCIOT.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCIOT.Etapa3.idTab + " .step").attr("class", "step red");
    Etapa2Aprovada();
}