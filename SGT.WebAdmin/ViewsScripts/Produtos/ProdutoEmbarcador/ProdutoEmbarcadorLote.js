/// <reference path="ProdutoEmbarcador.js" />

var _gridProdutoEmbarcadorLote;
var _lote;
//*******EVENTOS*******

var Lote = function () {
    this.Grid = PropertyEntity({ type: types.local });
};


function loadLotesProdutoEmbarcador() {
    _lote = new Lote();
    KoBindings(_lote, "knockoutLote");

    preencherLotesProdutoEmbarcado();
}


function preencherLotesProdutoEmbarcado() {

    var header = [{ data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "30%" },
        { data: "Numero", title: Localization.Resources.Gerais.Geral.Numero, width: "10%" },
        { data: "CodigoBarras", title: Localization.Resources.Produtos.ProdutoEmbarcador.CodigoDeBarras, width: "10%" },
        { data: "DataVencimento", title: Localization.Resources.Produtos.ProdutoEmbarcador.Vencimento, width: "12%" },
        { data: "QuantidadeLote", title: Localization.Resources.Produtos.ProdutoEmbarcador.QtdLote, width: "10%" },
        { data: "QuantidadeAtual", title: Localization.Resources.Produtos.ProdutoEmbarcador.QtdAtual, width: "10%" },
        { data: "DepositoPosicao", title: Localization.Resources.Produtos.ProdutoEmbarcador.Armazenamento, width: "20%" }];

    _gridProdutoEmbarcadorLote = new BasicDataTable(_lote.Grid.id, header);
    recarregarGridLotesProdutoEmbarcado();
}

function recarregarGridLotesProdutoEmbarcado() {
    var data = new Array();
    $.each(_produtoEmbarcador.Lotes.list, function (i, lote) {
        var obj = new Object();

        obj.Codigo = lote.Codigo.val;
        obj.Descricao = lote.Descricao.val;
        obj.Numero = lote.Numero.val;
        obj.CodigoBarras = lote.CodigoBarras.val;
        obj.DataVencimento = lote.DataVencimento.val;
        obj.QuantidadeLote = lote.QuantidadeLote.val;
        obj.QuantidadeAtual = lote.QuantidadeAtual.val;
        obj.DepositoPosicao = lote.DepositoPosicao.val;

        data.push(obj);
    });
    _gridProdutoEmbarcadorLote.CarregarGrid(data);
}