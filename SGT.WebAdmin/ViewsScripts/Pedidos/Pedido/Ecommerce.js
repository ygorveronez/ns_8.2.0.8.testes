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
/// <reference path="Pedido.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _ecommercePedido;

var EcommercePedido = function () {
    this.AlturaPedido = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.AlturaPedido.getFieldDescription(), getType: typesKnockout.decimal, visible: ko.observable(true) });
    this.LarguaPedido = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.LarguaPedido.getFieldDescription(),  getType: typesKnockout.decimal, visible: ko.observable(true) });
    this.ComprimentoPedido = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.ComprimentoPedido.getFieldDescription(), getType: typesKnockout.decimal, visible: ko.observable(true) });
    this.DiametroPedido = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.DiametroPedido.getFieldDescription(), getType: typesKnockout.decimal, visible: ko.observable(true) });
    this.CategoriaPrincipalProduto = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.CategoriaPrincipalProduto.getFieldDescription(), getType: typesKnockout.string, visible: ko.observable(true) });
    this.SerieNFe = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.SerieNFe.getFieldDescription(), getType: typesKnockout.string, visible: ko.observable(true) });
    this.ChaveAcessoNFe = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.ChaveAcessoNFe.getFieldDescription(), getType: typesKnockout.string, visible: ko.observable(true) });
    this.NaturezaGeralMercadorias = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.NaturezaGeralMercadorias.getFieldDescription(), getType: typesKnockout.string, visible: ko.observable(true) });
    this.TipoGeralMercadorias = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.TipoGeralMercadorias.getFieldDescription(), getType: typesKnockout.string, visible: ko.observable(true) });
    this.PrazoEntregaLoja = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.PrazoEntregaLoja.getFieldDescription(), getType: typesKnockout.int, visible: ko.observable(true) });
    this.TipoFrete = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.TipoFrete.getFieldDescription(), getType: typesKnockout.string, visible: ko.observable(true) });
    this.DataPagamentoPedido = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.DataPagamentoPedido.getFieldDescription(), getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.ModalidadeEntrega = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.ModalidadeEntrega.getFieldDescription(), getType: typesKnockout.string, visible: ko.observable(true) });
    this.CodigoTabelaFreteSistemaFIS = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.CodigoTabelaFreteSistemaFIS.getFieldDescription(), getType: typesKnockout.int, visible: ko.observable(true) });
    this.CFOPPredominanteNFe = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.CFOPPredominanteNFe.getFieldDescription(), getType: typesKnockout.string, visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadEcommercePedido() {
    _ecommercePedido = new EcommercePedido();
    KoBindings(_ecommercePedido, "knockoutEcommerce");
}

//*******MÉTODOS*******

function preencherDadosEcommerceSalvar(pedido) {
    let ecommerce = RetornarObjetoPesquisa(_ecommercePedido);
    $.extend(pedido, ecommerce);
}

function preecherEcommercePedido(dadosEcommerce) {
    PreencherObjetoKnout(_ecommercePedido, { Data: dadosEcommerce });
}

function limparCamposEcommercePedido() {
    LimparCampos(_ecommercePedido);
}