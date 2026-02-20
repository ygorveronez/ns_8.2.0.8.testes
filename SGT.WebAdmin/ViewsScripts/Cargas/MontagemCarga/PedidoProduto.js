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
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="Bloco.js" />
/// <reference path="Carga.js" />
/// <reference path="Carregamento.js" />
/// <reference path="CarregamentoCarga.js" />
/// <reference path="CarregamentoPedido.js" />
/// <reference path="Carregamentos.js" />
/// <reference path="CarregamentoTransporte.js" />
/// <reference path="DirecoesGoogleMaps.js" />
/// <reference path="Distancia.js" />
/// <reference path="GoogleMaps.js" />
/// <reference path="MontagemCarga.js" />
/// <reference path="OrigemDestino.js" />
/// <reference path="Pedido.js" />
/// <reference path="PedidosMapa.js" />
/// <reference path="Roteirizador.js" />
/// <reference path="SimulacaoFrete.js" />



//*******MAPEAMENTO KNOUCKOUT*******

var _pedidoProduto;
var _gridPedidoProduto;
var PedidoProduto = function () {
    this.Pedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Grid = PropertyEntity({ type: types.local });

}
//*******EVENTOS*******


function loadPedidoProduto() {
    _pedidoProduto = new PedidoProduto();
    KoBindings(_pedidoProduto, "knoutProdutos");
    _gridPedidoProduto = new GridView(_pedidoProduto.Grid.id, "PedidoProduto/Pesquisa", _pedidoProduto, null);
}

function carregarPedidoProduto(codigoPedido, callback) {
    _pedidoProduto.Pedido.val(codigoPedido);
    _gridPedidoProduto.CarregarGrid(callback);
}
