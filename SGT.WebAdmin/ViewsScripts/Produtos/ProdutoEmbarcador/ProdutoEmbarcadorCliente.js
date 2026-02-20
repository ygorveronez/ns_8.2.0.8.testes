/// <reference path="ProdutoEmbarcador.js" />

var _gridProdutoEmbarcadorCliente;
var _cliente;
//*******EVENTOS*******

var Cliente = function () {
    this.Grid = PropertyEntity({ type: types.local });
};

function loadClientesProdutoEmbarcador() {
    _cliente = new Cliente();
    KoBindings(_cliente, "knockoutCliente");

    preencherClientesProdutoEmbarcado();
}


function preencherClientesProdutoEmbarcado() {

    var header = [{ data: "Codigo", visible: false },
        { data: "CodigoBarras", title: Localization.Resources.Produtos.ProdutoEmbarcador.CodigoDeBarras, width: "50%" },
        { data: "Cliente", title: Localization.Resources.Produtos.ProdutoEmbarcador.Cliente, width: "50%" }];

    _gridProdutoEmbarcadorCliente = new BasicDataTable(_cliente.Grid.id, header);
    recarregarGridClientesProdutoEmbarcado();
}

function recarregarGridClientesProdutoEmbarcado() {
    var data = new Array();
    $.each(_produtoEmbarcador.Clientes.list, function (i, lote) {
        var obj = new Object();

        obj.Codigo = lote.Codigo.val;
        obj.CodigoBarras = lote.CodigoBarras.val;
        obj.Cliente = lote.Cliente.val;

        data.push(obj);
    });
    _gridProdutoEmbarcadorCliente.CarregarGrid(data);
}