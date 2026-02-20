/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _etapaContratoFrete;

var EtapaContratoFrete = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("50%") });

    this.Etapa1 = PropertyEntity({
        text: "Contrato de Frete", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("É onde se visualiza os dados do Contrato de Frete."),
        tooltipTitle: ko.observable("Contrato de Frete")
    });
    this.Etapa2 = PropertyEntity({
        text: "Aprovação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("Nessa etapa é possível acompanhar a aprovação."),
        tooltipTitle: ko.observable("Aprovação")
    });
}


//*******EVENTOS*******

function loadEtapas() {
    _etapaContratoFrete = new EtapaContratoFrete();
    KoBindings(_etapaContratoFrete, "knockoutEtapaContratoFrete");

    Etapa1Liberada();
}

function SetarEtapaInicio() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapaContratoFrete.Etapa1.idTab).click();
}

function SetarEtapa() {
    var situacao = _contratoFrete.SituacaoContratoFrete.val();

    if (situacao == EnumSituacaoContratoFrete.Aprovado || situacao == EnumSituacaoContratoFrete.Finalizada)
        Etapa2Aprovada();
    else if (situacao == EnumSituacaoContratoFrete.AgAprovacao)
        Etapa2Aguardando();
    else if (situacao == EnumSituacaoContratoFrete.SemRegra)
        Etapa2Aguardando();
    else if (situacao == EnumSituacaoContratoFrete.Rejeitado)
        Etapa2Reprovado();
    else if (situacao == EnumSituacaoContratoFrete.Cancelado)
        Etapa1Reprovado();
    else
        Etapa1Liberada();
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaContratoFrete.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaContratoFrete.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaContratoFrete.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaContratoFrete.Etapa1.idTab + " .step").attr("class", "step green");
}

function Etapa1Reprovado() {
    $("#" + _etapaContratoFrete.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaContratoFrete.Etapa1.idTab + " .step").attr("class", "step red");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    $("#" + _etapaContratoFrete.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaContratoFrete.Etapa2.idTab + " .step").attr("class", "step");
}

function Etapa2Aprovada() {
    $("#" + _etapaContratoFrete.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaContratoFrete.Etapa2.idTab + " .step").attr("class", "step green");
    Etapa1Aprovada();
}

function Etapa2Aguardando() {
    $("#" + _etapaContratoFrete.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaContratoFrete.Etapa2.idTab + " .step").attr("class", "step yellow");
    $("#" + _etapaContratoFrete.Etapa2.idTab).click();
    Etapa1Aprovada();
}

function Etapa2Reprovado() {
    $("#" + _etapaContratoFrete.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaContratoFrete.Etapa2.idTab + " .step").attr("class", "step red");
    $("#" + _etapaContratoFrete.Etapa2.idTab).click();
    Etapa1Aprovada();
}