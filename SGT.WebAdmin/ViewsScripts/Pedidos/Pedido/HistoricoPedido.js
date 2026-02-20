/// <reference path="Pedido.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridHistoricoPedido;
var _historicoPedido;

/*
 * Declaração das Classes
 */

var HistoricoPedido = function () {
    this.HistoricoPedido = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.OcorrenciasPedido, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadHistoricoPedido() {
    _historicoPedido = new HistoricoPedido();
    KoBindings(_historicoPedido, "knockoutHistoricoPedido");

    var header = [
        { data: "Codigo", visible: false },
        { data: "Usuario", title: Localization.Resources.Pedidos.Pedido.Usuario, width: "10%", className: "text-align-center" },
        { data: "CampoAlterado", title: Localization.Resources.Pedidos.Pedido.CampoAlterado, width: "20%", className: "text-align-left" },
        { data: "DataHora", title: Localization.Resources.Pedidos.Pedido.DataHora, width: "10%", className: "text-align-left" },
        { data: "ValorAnterior", title: Localization.Resources.Pedidos.Pedido.ValorAnterior, width: "30%", className: "text-align-left" },
        { data: "ValorAtual", title: Localization.Resources.Pedidos.Pedido.ValorAtual, width: "30%", className: "text-align-left" },
    ];

    _gridHistoricoPedido = new BasicDataTable(_historicoPedido.HistoricoPedido.idGrid, header, null);
    CarregarGridHistoricoPedido(new Array());
}

function CarregarGridHistoricoPedido(dataGrid) {
    if (dataGrid != null)
        _gridHistoricoPedido.CarregarGrid(dataGrid);
}