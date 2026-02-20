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
/// <reference path="../../Enumeradores/EnumSituacaoValePallet.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _etapaValePallet;

var EtapaValePallet = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("33%") });

    this.Etapa1 = PropertyEntity({
        text: "Lançamento", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("É onde informa os dados do lançamento."),
        tooltipTitle: ko.observable("Lançamento")
    });
    this.Etapa2 = PropertyEntity({
        text: "Devolução", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("É onde se acompanha a devolução."),
        tooltipTitle: ko.observable("Devolução")
    });
    this.Etapa3 = PropertyEntity({
        text: "Recolhimento", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable("É onde se informa os dados do recolhimento."),
        tooltipTitle: ko.observable("Recolhimento")
    });
}


//*******EVENTOS*******

function LoadEtapasValePallet() {
    _etapaValePallet = new EtapaValePallet();
    KoBindings(_etapaValePallet, "knockoutEtapasValePallet");
    
    SetarEtapaValePalletInicio();
}

function SetarEtapaValePalletInicio() {
    DesabilitarTodasEtapaValePallet();
    EtapaValePallet1Liberada();
    $("#" + _etapaValePallet.Etapa1.idTab).click();
}

function SetarEtapasValePallet() {
    var situacao = _valePallet.Situacao.val();

    if (situacao == EnumSituacaoValePallet.AgDevolucao)
        EtapaValePallet2Liberada();
    else if (situacao == EnumSituacaoValePallet.Cancelado)
        EtapaValePallet2Reprovada();
    else if (situacao == EnumSituacaoValePallet.AgFinalizacao)
            EtapaValePallet3Liberada();
    else if (situacao == EnumSituacaoValePallet.Finalizado)
        EtapaValePallet3Aprovada();
}

function DesabilitarTodasEtapaValePallet() {
    EtapaValePallet3Desabilitada();
}

//*******Etapa 1*******

function EtapaValePallet1Liberada() {
    $("#" + _etapaValePallet.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaValePallet.Etapa1.idTab + " .step").attr("class", "step yellow");
    EtapaValePallet3Desabilitada();
}

function EtapaValePallet1Aprovada() {
    $("#" + _etapaValePallet.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaValePallet.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function EtapaValePallet2Desabilitada() {
    $("#" + _etapaValePallet.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaValePallet.Etapa2.idTab + " .step").attr("class", "step");
}

function EtapaValePallet2Aprovada() {
    $("#" + _etapaValePallet.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaValePallet.Etapa2.idTab + " .step").attr("class", "step green");
    EtapaValePallet1Aprovada();
}

function EtapaValePallet2Reprovada() {
    $("#" + _etapaValePallet.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaValePallet.Etapa2.idTab + " .step").attr("class", "step red");
    EtapaValePallet1Aprovada();
}

function EtapaValePallet2Liberada() {
    $("#" + _etapaValePallet.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaValePallet.Etapa2.idTab + " .step").attr("class", "step yellow");
    EtapaValePallet1Aprovada();
}

//*******Etapa 3*******

function EtapaValePallet3Desabilitada() {
    $("#" + _etapaValePallet.Etapa3.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaValePallet.Etapa3.idTab + " .step").attr("class", "step");
    EtapaValePallet2Desabilitada();
}

function EtapaValePallet3Aprovada() {
    $("#" + _etapaValePallet.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaValePallet.Etapa3.idTab + " .step").attr("class", "step green");
    EtapaValePallet2Aprovada();
}

function EtapaValePallet3Liberada() {
    $("#" + _etapaValePallet.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaValePallet.Etapa3.idTab + " .step").attr("class", "step yellow");
    EtapaValePallet2Aprovada();
}