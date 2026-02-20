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
/// <reference path="../../Enumeradores/EnumSituacaoContratoFreteTransportador.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _etapaContratoTransportador;

var EtapaContratoTransportador = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("50%") });

    this.Etapa1 = PropertyEntity({
        text: "Contrato", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("Esta etapa é destinada ao dados do Contrato."),
        tooltipTitle: ko.observable("Contrato")
    });
    this.Etapa2 = PropertyEntity({
        text: "Aprovação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("Nessa etapa é possível acompanhar a aprovação."),
        tooltipTitle: ko.observable("Aprovação")
    });
    this.Etapa3 = PropertyEntity({
        text: "Integração", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable("Quando configurado, será gerado integrações"),
        tooltipTitle: ko.observable("Integração")
    });
}


//*******EVENTOS*******

function loadEtapasContratoTransportador() {
    _etapaContratoTransportador = new EtapaContratoTransportador();
    KoBindings(_etapaContratoTransportador, "knockoutEtapaContratoTransportador");
    Etapa1Liberada();
}

function SetarEtapaInicio() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapaContratoTransportador.Etapa1.idTab).click();
}

function SetarEtapasContratoTransportador() {
    var situacao = _contratoFreteTransportador.Situacao.val();

    if (situacao == EnumSituacaoContratoFreteTransportador.todos)
        Etapa1Liberada();
    else if (situacao == EnumSituacaoContratoFreteTransportador.AgAprovacao)
        Etapa2Liberada();
    else if (situacao == EnumSituacaoContratoFreteTransportador.Aprovado)
        Etapa2Aprovada();
    else if (situacao == EnumSituacaoContratoFreteTransportador.Rejeitado)
        Etapa2Reprovada();
    else if (situacao == EnumSituacaoContratoFreteTransportador.SemRegra)
        Etapa2Reprovada();
    else if (situacao == EnumSituacaoContratoFreteTransportador.Novo)
        Etapa1Aprovada();
    else if (situacao == EnumSituacaoContratoFreteTransportador.AgIntegracao)
        Etapa3Aguardando();
    else if (situacao == EnumSituacaoContratoFreteTransportador.ProblemaIntegracao)
        Etapa3Reprovada();
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
    Etapa3Desabilitada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaContratoTransportador.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaContratoTransportador.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaContratoTransportador.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaContratoTransportador.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    $("#" + _etapaContratoTransportador.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaContratoTransportador.Etapa2.idTab + " .step").attr("class", "step");
}

function Etapa2Aprovada() {
    $("#" + _etapaContratoTransportador.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaContratoTransportador.Etapa2.idTab + " .step").attr("class", "step green");
    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    $("#" + _etapaContratoTransportador.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaContratoTransportador.Etapa2.idTab + " .step").attr("class", "step red");
    Etapa1Aprovada();
}

function Etapa2Liberada() {
    $("#" + _etapaContratoTransportador.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaContratoTransportador.Etapa2.idTab + " .step").attr("class", "step yellow");
    $("#" + _etapaContratoTransportador.Etapa2.idTab).click();
    Etapa1Aprovada();
}

// ** integração ** //

function Etapa3Desabilitada() {
    _etapaContratoTransportador.Etapa3.eventClick = function () { };
    $("#" + _etapaContratoTransportador.Etapa3.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaContratoTransportador.Etapa3.idTab + " .step").attr("class", "step");
}

function Etapa3Liberada() {
    _etapaContratoTransportador.Etapa3.eventClick = recarregarContratoFreteTransportadorIntegracoes;
    $("#" + _etapaContratoTransportador.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaContratoTransportador.Etapa3.idTab + " .step").attr("class", "step yellow");
    Etapa2Aprovada();
}

function Etapa3Aguardando() {
    _etapaContratoTransportador.Etapa3.eventClick = recarregarContratoFreteTransportadorIntegracoes;
    $("#" + _etapaContratoTransportador.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaContratoTransportador.Etapa3.idTab + " .step").attr("class", "step yellow");
    $("#" + _etapaContratoTransportador.Etapa3.idTab).click();
    Etapa2Aprovada();
}

function Etapa3Aprovada() {
    _etapaContratoTransportador.Etapa3.eventClick = recarregarContratoFreteTransportadorIntegracoes;
    $("#" + _etapaContratoTransportador.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaContratoTransportador.Etapa3.idTab + " .step").attr("class", "step green");
    $("#" + _etapaContratoTransportador.Etapa3.idTab).click();
    Etapa2Aprovada();
}

function Etapa3Reprovada() {
    _etapaContratoTransportador.Etapa3.eventClick = recarregarContratoFreteTransportadorIntegracoes;
    $("#" + _etapaContratoTransportador.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaContratoTransportador.Etapa3.idTab + " .step").attr("class", "step red");
    $("#" + _etapaContratoTransportador.Etapa3.idTab).click();
    Etapa2Aprovada();
}