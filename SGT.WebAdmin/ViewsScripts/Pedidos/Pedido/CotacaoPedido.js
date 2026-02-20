/// <reference path="Pedido.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridCotacaoPedido;
var _cotacaoPedido;

/*
 * Declaração das Classes
 */

var CotacaoPedido = function () {
    this.Cotacao = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.Cotacoes, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadCotacaoPedido() {
    _cotacaoPedido = new CotacaoPedido();
    KoBindings(_cotacaoPedido, "knockoutCotacaoPedido");

    var header = [
        { data: "Codigo", visible: false },
        { data: "IDOferta", title: Localization.Resources.Pedidos.Pedido.IdOferta, width: "6%", className: "text-align-right" },
        { data: "Transportador", title: Localization.Resources.Pedidos.Pedido.Transportadora, width: "30%", className: "text-align-left" },
        { data: "Servico", title: Localization.Resources.Pedidos.Pedido.Servico, width: "15%", className: "text-align-left", visible: !_CONFIGURACAO_TMS.HabilitarFluxoPedidoEcommerce },
        { data: "DataColetaPrevista", title: Localization.Resources.Pedidos.PedidoDataColetaPrevisa, width: "10%", className: "text-align-center", visible: !_CONFIGURACAO_TMS.HabilitarFluxoPedidoEcommerce },
        { data: "DataPrazoEntrega", title: Localization.Resources.Pedidos.Pedido.DataPrazoColeta, width: "10%", className: "text-align-center" },
        { data: "PrecoFrete", title: Localization.Resources.Pedidos.Pedido.PrecoDoFrete, width: "7%", className: "text-align-right", visible: !_CONFIGURACAO_TMS.HabilitarFluxoPedidoEcommerce },
        { data: "ValorFrete", title: Localization.Resources.Pedidos.Pedido.ValorDoFrete, width: "7%", className: "text-align-right" },
        { data: "Prazo", title: Localization.Resources.Pedidos.Pedido.Prazo, width: "5%", className: "text-align-right" },
        { data: "KMTotal", title: Localization.Resources.Pedidos.Pedido.DistanciaRaioKM, width: "10%", className: "text-align-right" },
        { data: "Validade", title: Localization.Resources.Pedidos.Pedido.Validade, width: "10%", className: "text-align-center", visible: !_CONFIGURACAO_TMS.HabilitarFluxoPedidoEcommerce },
        { data: "ValorFreteTonelada", title: Localization.Resources.Pedidos.Pedido.ValorFreteTon, width: "10%", className: "text-align-center", visible: !_CONFIGURACAO_TMS.HabilitarFluxoPedidoEcommerce },
        { data: "AplicarICMS", title: Localization.Resources.Pedidos.Pedido.AplicarICMS, width: "10%", className: "text-align-center", visible: !_CONFIGURACAO_TMS.HabilitarFluxoPedidoEcommerce },
        { data: "CanalVenda", title: Localization.Resources.Pedidos.Pedido.CanalVenda, width: "10%", className: "text-align-center" },
    ];
    _gridCotacaoPedido = new BasicDataTable(_cotacaoPedido.Cotacao.idGrid, header);
    carregarGridCotacaoPedido(new Array());
}

function carregarGridCotacaoPedido(dataGrid) {
    _gridCotacaoPedido.CarregarGrid(dataGrid);
}
