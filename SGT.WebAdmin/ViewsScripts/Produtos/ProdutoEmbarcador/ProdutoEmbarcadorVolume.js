/// <reference path="ProdutoEmbarcador.js" />

var _gridProdutoEmbarcadorVolume;
var _volume;

//*******EVENTOS*******

var Volume = function () {
    this.Grid = PropertyEntity({ type: types.local });
};

function loadVolumesProdutoEmbarcador() {
    _volume = new Volume();
    KoBindings(_volume, "knockoutVolume");

    preencherVolumesProdutoEmbarcado();
}


function preencherVolumesProdutoEmbarcado() {

    var header = [{ data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Produtos.ProdutoEmbarcador.NSEntrega, width: "10%" },
        { data: "Numero", title: Localization.Resources.Produtos.ProdutoEmbarcador.NumeroNF, width: "10%" },
        { data: "QuantidadeAtual", title: Localization.Resources.Produtos.ProdutoEmbarcador.QtdAtual, width: "10%" },
        { data: "CodigoBarras", title: Localization.Resources.Produtos.ProdutoEmbarcador.CodigoDeBarras, width: "10%" },
        { data: "Remetente", title: Localization.Resources.Produtos.ProdutoEmbarcador.Remetente, width: "20%" },
        { data: "DepositoPosicao", title: Localization.Resources.Produtos.ProdutoEmbarcador.Armazenamento, width: "20%" }];

    _gridProdutoEmbarcadorVolume = new BasicDataTable(_volume.Grid.id, header);
    recarregarGridVolumesProdutoEmbarcado();
}

function recarregarGridVolumesProdutoEmbarcado() {
    var data = new Array();
    $.each(_produtoEmbarcador.Volumes.list, function (i, lote) {
        var obj = new Object();

        obj.Codigo = lote.Codigo.val;
        obj.Descricao = lote.Descricao.val;
        obj.Numero = lote.Numero.val;
        obj.QuantidadeAtual = lote.QuantidadeAtual.val;
        obj.CodigoBarras = lote.CodigoBarras.val;
        obj.Remetente = lote.Remetente.val;
        obj.DepositoPosicao = lote.DepositoPosicao.val;

        data.push(obj);
    });
    _gridProdutoEmbarcadorVolume.CarregarGrid(data);
}