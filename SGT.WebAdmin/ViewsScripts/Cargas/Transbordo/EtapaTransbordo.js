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
/// <reference path="../../Enumeradores/EnumSituacaoTransbordo.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _etapaTransbordo;

var EtapaTransbordo = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable(verificarTamanhoEtapa) });

    this.Etapa1 = PropertyEntity({
        text: Localization.Resources.Cargas.Transbordo.DadosTransbordos, type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("Transbordo")
    });
    this.Etapa2 = PropertyEntity({
        text: Localization.Resources.Cargas.Transbordo.Integracao, type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("Integração")
    });
}

//*******EVENTOS*******

function LoadEtapaTransbordo() {
    _etapaTransbordo = new EtapaTransbordo();
    KoBindings(_etapaTransbordo, "knockoutEtapaTransbordo");
    Etapa1Liberada();
}

function SetarEtapaInicioTransbordo() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapaTransbordo.Etapa1.idTab)[0].click();
}

function SetarEtapaTransbordo() {
    var situacaoTransbordo = _transbordo.SituacaoTransbordo.val();

    if (situacaoTransbordo == EnumSituacaoTransbordo.EmTransporte)
        Etapa2Aprovada();
    else if (situacaoTransbordo == EnumSituacaoTransbordo.Finalizado)
        Etapa2Aprovada();
    else if (situacaoTransbordo == EnumSituacaoTransbordo.Cancelado)
        Etapa2Aprovada();
    else if (situacaoTransbordo == EnumSituacaoTransbordo.AgIntegracao)
        Etapa2Aguardando();
    else if (situacaoTransbordo == EnumSituacaoTransbordo.FalhaIntegracao)
        Etapa2Reprovada();
    else
        Etapa1Aguardando();
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
}

function DefinirTab() {
    if (_transbordo.SituacaoTransbordo.val() == EnumSituacaoTransbordo.AgIntegracao) {
        $("#" + _etapaTransbordo.Etapa2.idTab).tab("show");
        _etapaTransbordo.Etapa2.eventClick();
    }

    else if (_transbordo.SituacaoTransbordo.val() == EnumSituacaoTransbordo.FalhaIntegracao) {
        $("#" + _etapaTransbordo.Etapa2.idTab).tab("show");
        _etapaTransbordo.Etapa2.eventClick();
    }

    else {
        $("#" + _etapaTransbordo.Etapa1.idTab).tab("show");
        _etapaTransbordo.Etapa1.eventClick();
    }


}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaTransbordo.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaTransbordo.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aguardando() {
    $("#" + _etapaTransbordo.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaTransbordo.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaTransbordo.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaTransbordo.Etapa1.idTab + " .step").attr("class", "step green");
}

function Etapa1Reprovada() {
    $("#" + _etapaTransbordo.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaTransbordo.Etapa1.idTab + " .step").attr("class", "step red");
    Etapa2Desabilitada();
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    _etapaTransbordo.Etapa2.eventClick = function () { };
    $("#" + _etapaTransbordo.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaTransbordo.Etapa2.idTab + " .step").attr("class", "step");
}

function Etapa2Liberada() {
    _etapaTransbordo.Etapa2.eventClick = BuscarDadosIntegracoesTransbordo;
    $("#" + _etapaTransbordo.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaTransbordo.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
}

function Etapa2Aguardando() {
    _etapaTransbordo.Etapa2.eventClick = BuscarDadosIntegracoesTransbordo;
    $("#" + _etapaTransbordo.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaTransbordo.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
}

function Etapa2Aprovada() {
    _etapaTransbordo.Etapa2.eventClick = BuscarDadosIntegracoesTransbordo;
    $("#" + _etapaTransbordo.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaTransbordo.Etapa2.idTab + " .step").attr("class", "step green");
    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    _etapaTransbordo.Etapa2.eventClick = BuscarDadosIntegracoesTransbordo;
    $("#" + _etapaTransbordo.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaTransbordo.Etapa2.idTab + " .step").attr("class", "step red");
    Etapa1Aprovada();
}

// ********* Funções ********* //

function verificarTamanhoEtapa() {
    return "50%";
}