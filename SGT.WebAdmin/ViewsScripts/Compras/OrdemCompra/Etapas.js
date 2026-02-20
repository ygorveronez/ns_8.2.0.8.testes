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
/// <reference path="../../Enumeradores/EnumSituacaoOrdemCompra.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _etapaOrdemCompra;

var EtapaOrdemCompra = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("50%") });

    this.Etapa1 = PropertyEntity({
        text: "Ordem de Compra", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("É onde informa os dados da ordem."),
        tooltipTitle: ko.observable("Ordem de Compra")
    });
    this.Etapa2 = PropertyEntity({
        text: "Aprovação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("Nessa etapa é possível acompanhar a aprovação da ordem."),
        tooltipTitle: ko.observable("Aprovação")
    });
};


//*******EVENTOS*******

function LoadEtapasOrdemCompra() {
    _etapaOrdemCompra = new EtapaOrdemCompra();
    KoBindings(_etapaOrdemCompra, "knockoutEtapasOrdemCompra");
    Etapa1Liberada();
    SetarEtapaInicioOrdemCompra();
}

function SetarEtapaInicioOrdemCompra() {
    DesabilitarTodasEtapasOrdemCompra();
    Etapa1Liberada();
    $("#" + _etapaOrdemCompra.Etapa1.idTab).click();
}

function SetarEtapasOrdemCompra() {
    var situacao = _ordemCompra.Situacao.val();

    if (situacao === EnumSituacaoOrdemCompra.Aberta)
        Etapa1Liberada();
    else if (situacao === EnumSituacaoOrdemCompra.AgAprovacao)
        Etapa2Aguardando();
    else if (situacao === EnumSituacaoOrdemCompra.Aprovada)
        Etapa2Aprovada();
    else if (situacao === EnumSituacaoOrdemCompra.Rejeitada || situacao === EnumSituacaoOrdemCompra.Cancelada)
        Etapa2Reprovada();
    else if (situacao === EnumSituacaoOrdemCompra.SemRegra)
        Etapa2Aguardando();
}

function DesabilitarTodasEtapasOrdemCompra() {
    Etapa2Desabilitada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaOrdemCompra.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOrdemCompra.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaOrdemCompra.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOrdemCompra.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    $("#" + _etapaOrdemCompra.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaOrdemCompra.Etapa2.idTab + " .step").attr("class", "step");
}

function Etapa2Aprovada() {
    $("#" + _etapaOrdemCompra.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOrdemCompra.Etapa2.idTab + " .step").attr("class", "step green");
    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    $("#" + _etapaOrdemCompra.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOrdemCompra.Etapa2.idTab + " .step").attr("class", "step red");
    Etapa1Aprovada();
}

function Etapa2Aguardando() {
    $("#" + _etapaOrdemCompra.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOrdemCompra.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
}

