/// <reference path="../../Consultas/Filial.js" />

var _pesquisaPedido;
var _gridPedido;
var _dataGridCarregada = [];

var PesquisaPedido = function () {
    this.NumeroPedido = PropertyEntity({ val: ko.observable(""), text: "Número Pedido: " });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            RecarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}

//*******EVENTOS*******
function loadPedidoProdutosCarregamentos() {
    _pesquisaPedido = new PesquisaPedido();
    KoBindings(_pesquisaPedido, "knockoutPesquisaPedido", false, _pesquisaPedido.Pesquisar.id);

    //Filial
    new BuscarFilial(_pesquisaPedido.Filial);

    BuscarPedidoProdutosCarregamentos();
}

function BuscarPedidoProdutosCarregamentos() {
    _gridPedido = new GridView(_pesquisaPedido.Pesquisar.idGrid, "PedidoProduto/DetalhesPedidoProdutosCarregamentos", _pesquisaPedido, null, null, 25);
    RecarregarGrid();
}

function GridCarregada(data) {
    _dataGridCarregada = data.data;
}

function RecarregarGrid(cb) {
    _gridPedido.CarregarGrid(function (data) {
        GridCarregada(data);

        if (cb != null) cb();
    });
}
