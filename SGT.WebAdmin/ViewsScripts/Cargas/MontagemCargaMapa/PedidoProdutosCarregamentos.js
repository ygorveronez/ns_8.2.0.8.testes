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
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="Bloco.js" />
/// <reference path="Carregamento.js" />
/// <reference path="CarregamentoPedido.js" />
/// <reference path="Carregamentos.js" />
/// <reference path="CarregamentoTransporte.js" />
/// <reference path="DirecoesGoogleMaps.js" />
/// <reference path="Distancia.js" />
/// <reference path="GoogleMaps.js" />
/// <reference path="MontagemCarga.js" />
/// <reference path="Pedido.js" />
/// <reference path="PedidosMapa.js" />
/// <reference path="Roteirizador.js" />
/// <reference path="SimulacaoFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pedidoProdutosCarregamentos ;
var _gridPedidoProdutosCarregamentos;

// Será apresentado nos detalhes do pedido (Modal todos os carregamentos dos pedidos produtos...)
var PedidoProdutosCarregamentos = function () {
    this.Pedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Grid = PropertyEntity({ id: guid() });
}
//*******EVENTOS*******


function loadPedidoProdutosCarregamentos() {    
    _pedidoProdutosCarregamentos = new PedidoProdutosCarregamentos();
    KoBindings(_pedidoProdutosCarregamentos, "knoutDetalhePedidoProdutosCarregamentos");    
    _gridPedidoProdutosCarregamentos = new GridView(_pedidoProdutosCarregamentos.Grid.id, "PedidoProduto/DetalhesPedidoProdutosCarregamentos", _pedidoProdutosCarregamentos, null);
}

function carregarPedidoProdutosCarregamentos(codigoPedido, callback) {
    _pedidoProdutosCarregamentos.Pedido.val(codigoPedido);
    _gridPedidoProdutosCarregamentos.CarregarGrid(callback);
}