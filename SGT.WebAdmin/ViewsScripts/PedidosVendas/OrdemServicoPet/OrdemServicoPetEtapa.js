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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="OrdemServicoPet.js" />
/// <reference path="OrdemServicoPetItens.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _etapaPedidoVenda;
var _etapaAtual;

var EtapaPedidoVenda = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("50%") });

    this.Etapa1 = PropertyEntity({
        text: "Nova Ordem de Serviço", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaNovoPedidoClick,
        step: ko.observable(1),
        tooltip: ko.observable("É onde se inicia uma ordem de serviço."),
        tooltipTitle: ko.observable("Nova Ordem de Serviço")
    });
    this.Etapa2 = PropertyEntity({
        text: "Itens Ordem de Serviço", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaItensPedidoClick,
        step: ko.observable(2),
        tooltip: ko.observable("É onde serão lançados os produtos e serviços da ordem de serviço."),
        tooltipTitle: ko.observable("Itens da Ordem de Serviço")
    });
}

function loadEtapaPedidoVenda() {
    _etapaPedidoVenda = new EtapaPedidoVenda();
    KoBindings(_etapaPedidoVenda, "knockoutEtapa");

    $("#" + _etapaPedidoVenda.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaPedidoVenda.Etapa1.idTab + " .step").attr("class", "step lightgreen");
    $("#" + _etapaPedidoVenda.Etapa1.idTab).click();
}

function etapaNovoPedidoClick(e, sender) {
    _etapaAtual = 1;
    PosicionarEtapa();
}

function etapaItensPedidoClick(e, sender) {
    _etapaAtual = 2;
    PosicionarEtapa()
}

function PosicionarEtapa() {
    LimparOcultarAbas();

    $("#" + _etapaPedidoVenda.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaPedidoVenda.Etapa2.idTab).attr("data-bs-toggle", "tab");

    if (_etapaAtual == 1) {
        $("#" + _etapaPedidoVenda.Etapa1.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaPedidoVenda.Etapa1.idTab + " .step").attr("class", "step lightgreen");
    } else if (_etapaAtual == 2) {
        $("#" + _etapaPedidoVenda.Etapa1.idTab + " .step").attr("class", "step green");

        $("#" + _etapaPedidoVenda.Etapa2.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaPedidoVenda.Etapa2.idTab + " .step").attr("class", "step lightgreen");
    }
}

function LimparOcultarAbas() {
    $("#" + _etapaPedidoVenda.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaPedidoVenda.Etapa1.idTab + " .step").attr("class", "step");

    $("#" + _etapaPedidoVenda.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaPedidoVenda.Etapa2.idTab + " .step").attr("class", "step");
}