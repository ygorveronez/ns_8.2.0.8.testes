/// <reference path="NFS.js" />
/// <reference path="CTe.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCancelamentoDocumentoCarga.js" />
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
/// <reference path="../../Enumeradores/EnumTipoCancelamentoCarga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _etapaCancelamento;

var EtapaCancelamento = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("33.33%") });

    this.Etapa1 = PropertyEntity({
        text: "Cancelamento", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("É onde se gera o cancelamento."),
        tooltipTitle: ko.observable("Cancelamento")
    });
    this.Etapa2 = PropertyEntity({
        text: "NFS", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("É a etapa de cancelamento da NFS."),
        tooltipTitle: ko.observable("NFS")
    });
    this.Etapa3 = PropertyEntity({
        text: "Integrações", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable("É a etapa de integrações do cancelamento da NFS."),
        tooltipTitle: ko.observable("Integrações")
    });
};

//*******EVENTOS*******

function LoadEtapaCancelamento() {
    _etapaCancelamento = new EtapaCancelamento();
    KoBindings(_etapaCancelamento, "knockoutEtapaCancelamento");
    Etapa1Liberada();
}

function SetarEtapaInicioCancelamento() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapaCancelamento.Etapa1.idTab).click();
}

function SetarEtapaCancelamento() {
    var situacaoCancelamento = _cancelamento.Situacao.val();

    if (situacaoCancelamento == EnumSituacaoNFSManualCancelamento.EmCancelamento)
        Etapa2Aguardando();
    else if (situacaoCancelamento == EnumSituacaoNFSManualCancelamento.Cancelada)
        Etapa3Aprovada();
    else if (situacaoCancelamento == EnumSituacaoNFSManualCancelamento.CancelamentoRejeitado)
        Etapa2Reprovada();
    else if (situacaoCancelamento == EnumSituacaoNFSManualCancelamento.IntegracaoRejeitada)
        Etapa3Reprovada();
    else if (situacaoCancelamento == EnumSituacaoNFSManualCancelamento.AgIntegracao)
        Etapa3Aguardando();
    else {
        SetarEtapaInicioCancelamento();
    }
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaCancelamento.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaCancelamento.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    _etapaCancelamento.Etapa2.eventClick = function () { };
    $("#" + _etapaCancelamento.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaCancelamento.Etapa2.idTab + " .step").attr("class", "step");
    $("#" + _etapaCancelamento.Etapa1.idTab).click();
}

function Etapa2Liberada() {
    _etapaCancelamento.Etapa2.eventClick = ConsultarNFSsCarga;
    $("#" + _etapaCancelamento.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
}

function Etapa2Aguardando() {
    _etapaCancelamento.Etapa2.eventClick = ConsultarNFSsCarga;
    $("#" + _etapaCancelamento.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
    $("#" + _etapaCancelamento.Etapa2.idTab).click();
}

function Etapa2Aprovada() {
    _etapaCancelamento.Etapa2.eventClick = ConsultarNFSsCarga;
    $("#" + _etapaCancelamento.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa2.idTab + " .step").attr("class", "step green");
    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    _etapaCancelamento.Etapa2.eventClick = ConsultarNFSsCarga;
    $("#" + _etapaCancelamento.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa2.idTab + " .step").attr("class", "step red");
    Etapa1Aprovada();
}

//*******Etapa 3*******

function Etapa3Desabilitada() {
    _etapaCancelamento.Etapa3.eventClick = function () { };
    $("#" + _etapaCancelamento.Etapa3.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaCancelamento.Etapa3.idTab + " .step").attr("class", "step");
    $("#" + _etapaCancelamento.Etapa2.idTab).click();
}

function Etapa3Liberada() {
    _etapaCancelamento.Etapa3.eventClick = BuscarDadosIntegracoesNFSManualCancelamento();
    $("#" + _etapaCancelamento.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa3.idTab + " .step").attr("class", "step yellow");
    Etapa2Aprovada();
}

function Etapa3Aguardando() {
    _etapaCancelamento.Etapa3.eventClick = BuscarDadosIntegracoesNFSManualCancelamento;
    $("#" + _etapaCancelamento.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa3.idTab + " .step").attr("class", "step yellow");
    Etapa2Aprovada();
    $("#" + _etapaCancelamento.Etapa3.idTab).click();
}

function Etapa3Aprovada() {
    _etapaCancelamento.Etapa3.eventClick = BuscarDadosIntegracoesNFSManualCancelamento;
    $("#" + _etapaCancelamento.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa3.idTab + " .step").attr("class", "step green");
    Etapa2Aprovada();
}

function Etapa3Reprovada() {
    _etapaCancelamento.Etapa3.eventClick = BuscarDadosIntegracoesNFSManualCancelamento;
    $("#" + _etapaCancelamento.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa3.idTab + " .step").attr("class", "step red");
    Etapa2Aprovada();
}
