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
/// <reference path="OrdemServicoVenda.js" />
/// <reference path="OrdemServicoVendaPecas.js" />
/// <reference path="OrdemServicoVendaMaoObra.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _etapaOrdemServicoVenda;
var _etapaAtual;

var EtapaOrdemServicoVenda = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("33%") });

    this.Etapa1 = PropertyEntity({
        text: "Nova Ordem", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaNovaOrdemClick,
        step: ko.observable(1),
        tooltip: ko.observable("É onde se inicia um pedido de venda."),
        tooltipTitle: ko.observable("Nova Ordem")
    });
    this.Etapa2 = PropertyEntity({
        text: "Peças", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaPecasClick,
        step: ko.observable(2),
        tooltip: ko.observable("É onde serão lançados as peças da ordem."),
        tooltipTitle: ko.observable("Peças")
    });
    this.Etapa3 = PropertyEntity({
        text: "Mão de Obra", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaMaodeObrasClick,
        step: ko.observable(3),
        tooltip: ko.observable("É onde serão lançados as mão de obras da ordem."),
        tooltipTitle: ko.observable("Mão de Obra")
    });
}

//*******EVENTOS*******

function loadEtapaOrdemServicoVenda() {
    _etapaOrdemServicoVenda = new EtapaOrdemServicoVenda();
    KoBindings(_etapaOrdemServicoVenda, "knockoutEtapaOrdemServicoVenda");

    $("#" + _etapaOrdemServicoVenda.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOrdemServicoVenda.Etapa1.idTab + " .step").attr("class", "step lightgreen");
    $("#" + _etapaOrdemServicoVenda.Etapa1.idTab).click();
}

function etapaNovaOrdemClick(e, sender) {
    _etapaAtual = 1;
    PosicionarEtapa();
}

function etapaPecasClick(e, sender) {
    _etapaAtual = 2;
    PosicionarEtapa();
}

function etapaMaodeObrasClick(e, sender) {
    _etapaAtual = 3;
    PosicionarEtapa();
}

//*******MÉTODOS*******

function PosicionarEtapa() {
    LimparOcultarAbas();

    $("#" + _etapaOrdemServicoVenda.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOrdemServicoVenda.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOrdemServicoVenda.Etapa3.idTab).attr("data-bs-toggle", "tab");

    if (_etapaAtual == 1) {
        $("#" + _etapaOrdemServicoVenda.Etapa1.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaOrdemServicoVenda.Etapa1.idTab + " .step").attr("class", "step lightgreen");
    } else if (_etapaAtual == 2) {
        $("#" + _etapaOrdemServicoVenda.Etapa1.idTab + " .step").attr("class", "step green");

        $("#" + _etapaOrdemServicoVenda.Etapa2.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaOrdemServicoVenda.Etapa2.idTab + " .step").attr("class", "step lightgreen");
    } else if (_etapaAtual == 3) {
        $("#" + _etapaOrdemServicoVenda.Etapa1.idTab + " .step").attr("class", "step green");
        $("#" + _etapaOrdemServicoVenda.Etapa2.idTab + " .step").attr("class", "step green");

        $("#" + _etapaOrdemServicoVenda.Etapa3.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaOrdemServicoVenda.Etapa3.idTab + " .step").attr("class", "step lightgreen");
    }
}

function LimparOcultarAbas() {
    $("#" + _etapaOrdemServicoVenda.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOrdemServicoVenda.Etapa1.idTab + " .step").attr("class", "step");

    $("#" + _etapaOrdemServicoVenda.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOrdemServicoVenda.Etapa2.idTab + " .step").attr("class", "step");

    $("#" + _etapaOrdemServicoVenda.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaOrdemServicoVenda.Etapa3.idTab + " .step").attr("class", "step");
}