/// <reference path="SolicitacaoAvaria.js" />
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
/// <reference path="../../Enumeradores/EnumSituacaoControleReajusteFretePlanilha.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _etapaControleReajusteFretePlanilha;

var EtapaControleReajusteFretePlanilha = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("33%") });

    this.Etapa1 = PropertyEntity({
        text: "Planilhas Reajuste", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("É onde se informa os dados e selecionado a planilha."),
        tooltipTitle: ko.observable("Planilhas Reajuste")
    });
    this.Etapa2 = PropertyEntity({
        text: "Aprovação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("Nessa etapa é possível acompanhar a aprovação."),
        tooltipTitle: ko.observable("Aprovação")
    });
    this.Etapa3 = PropertyEntity({
        text: "Finalização", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable("Etapa que finaliza o reajuste."),
        tooltipTitle: ko.observable("Finalização")
    });
}


//*******EVENTOS*******

function loadEtapas() {
    _etapaControleReajusteFretePlanilha = new EtapaControleReajusteFretePlanilha();
    KoBindings(_etapaControleReajusteFretePlanilha, "knockoutEtapaControleReajusteFretePlanilha");

    Etapa1Liberada();
}

function SetarEtapaInicio() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapaControleReajusteFretePlanilha.Etapa1.idTab).click();
}

function SetarEtapa() {
    var situacao = _controleReajusteFretePlanilha.Situacao.val();
    
    if (situacao == EnumSituacaoControleReajusteFretePlanilha.AgAprovacao)
        Etapa2Aguardando();
    else if (situacao == EnumSituacaoControleReajusteFretePlanilha.SemRegra)
        Etapa2Aguardando();
    else if (situacao == EnumSituacaoControleReajusteFretePlanilha.Rejeitado)
        Etapa2Reprovado();
    else if (situacao == EnumSituacaoControleReajusteFretePlanilha.Cancelado)
        Etapa1Reprovado();
    else if (situacao == EnumSituacaoControleReajusteFretePlanilha.Aprovado)
        Etapa3Aguardando();
    else if (situacao == EnumSituacaoControleReajusteFretePlanilha.Finalizado)
        Etapa3Aprovada();
    else
        Etapa1Liberada();
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
    Etapa3Desabilitada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaControleReajusteFretePlanilha.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaControleReajusteFretePlanilha.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaControleReajusteFretePlanilha.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaControleReajusteFretePlanilha.Etapa1.idTab + " .step").attr("class", "step green");
}

function Etapa1Reprovado() {
    $("#" + _etapaControleReajusteFretePlanilha.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaControleReajusteFretePlanilha.Etapa1.idTab + " .step").attr("class", "step red");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    $("#" + _etapaControleReajusteFretePlanilha.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaControleReajusteFretePlanilha.Etapa2.idTab + " .step").attr("class", "step");
    Etapa3Desabilitada()
}

function Etapa2Aprovada() {
    $("#" + _etapaControleReajusteFretePlanilha.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaControleReajusteFretePlanilha.Etapa2.idTab + " .step").attr("class", "step green");
    Etapa1Aprovada();
}

function Etapa2Aguardando() {
    $("#" + _etapaControleReajusteFretePlanilha.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaControleReajusteFretePlanilha.Etapa2.idTab + " .step").attr("class", "step yellow");
    $("#" + _etapaControleReajusteFretePlanilha.Etapa2.idTab).click();
    Etapa1Aprovada();
}

function Etapa2Reprovado() {
    $("#" + _etapaControleReajusteFretePlanilha.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaControleReajusteFretePlanilha.Etapa2.idTab + " .step").attr("class", "step red");
    $("#" + _etapaControleReajusteFretePlanilha.Etapa2.idTab).click();
    Etapa1Aprovada();
}



//*******Etapa 3*******

function Etapa3Desabilitada() {
    $("#" + _etapaControleReajusteFretePlanilha.Etapa3.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaControleReajusteFretePlanilha.Etapa3.idTab + " .step").attr("class", "step");
}

function Etapa3Aguardando() {
    $("#" + _etapaControleReajusteFretePlanilha.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaControleReajusteFretePlanilha.Etapa3.idTab + " .step").attr("class", "step yellow");
    $("#" + _etapaControleReajusteFretePlanilha.Etapa3.idTab).click();
    Etapa2Aprovada();
}

function Etapa3Aprovada() {
    $("#" + _etapaControleReajusteFretePlanilha.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaControleReajusteFretePlanilha.Etapa3.idTab + " .step").attr("class", "step green");
    Etapa2Aprovada();
}

function Etapa3Liberada() {
    $("#" + _etapaControleReajusteFretePlanilha.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaControleReajusteFretePlanilha.Etapa3.idTab + " .step").attr("class", "step blue");
    Etapa2Aprovada();
}