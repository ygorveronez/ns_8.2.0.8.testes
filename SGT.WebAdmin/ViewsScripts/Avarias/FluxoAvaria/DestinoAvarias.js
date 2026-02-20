
//*******MAPEAMENTO KNOUCKOUT*******

var _produtoAvaria;
var _gridProdutoAvaria;

var ProdutoAvaria = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.ProdutosAvariados = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), idGrid: guid() });
};

//*******EVENTOS*******

function LoadDestinoAvarias() {
    _produtoAvaria = new ProdutoAvaria();
    KoBindings(_produtoAvaria, "knockoutProdutoAvaria");

    LoadGridProdutoAvaria();

    LoadDestinoAvariasProduto();
}

////*******MÉTODOS*******

function CarregarDestinoAvarias() {
    executarReST("FluxoAvaria/BuscarProdutosAvariados", { Codigo: _fluxoAvaria.Lote.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                PreencherObjetoKnout(_produtoAvaria, r);
                RecarregarGridProdutoAvaria();
                BuscarDestinoAvariasProduto();
                CarregarDadosDestinoAvariasProduto();
                
                var codigos = r.Data.ProdutosAvariados.map(c => c.CodigoProdutoEmbarcador);
                new BuscarProdutos(_destinoAvariasProduto.Produto, null, null, null, null, null, null, null, null, null, JSON.stringify(codigos));
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function LoadGridProdutoAvaria() {
    var header = [
        { data: "Codigo", visible: false },
        { data: "ProdutoEmbarcador", title: "Produto", width: "40%" },
        { data: "NotaFiscal", title: "Nota Fiscal", width: "15%" },
        { data: "UnidadesAvariadas", title: "Un. Avariadas", width: "15%" }
    ];

    _gridProdutoAvaria = new BasicDataTable(_produtoAvaria.Grid.id, header);

    RecarregarGridProdutoAvaria();
}

function RecarregarGridProdutoAvaria() {
    var data = new Array();

    $.each(_produtoAvaria.ProdutosAvariados.list, function (i, item) {
        var itemGrid = new Object();

        itemGrid.Codigo = item.Codigo.val;
        itemGrid.ProdutoEmbarcador = item.ProdutoEmbarcador.val;
        itemGrid.NotaFiscal = item.NotaFiscal.val;
        itemGrid.UnidadesAvariadas = item.UnidadesAvariadas.val;

        data.push(itemGrid);
    });

    _gridProdutoAvaria.CarregarGrid(data);
}