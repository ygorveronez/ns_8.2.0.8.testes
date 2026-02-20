var _gridProdutoEmbarcador = null;

function LoadProdutoEmbarcador() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirProdutoEmbarcadorClick(_regraICMS.AdicionarProdutoEmbarcador, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridProdutoEmbarcador = new BasicDataTable(_regraICMS.GridProdutoEmbarcador.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    //new BuscarProdutos(_regraICMS.AdicionarProdutoEmbarcador, null, _regraICMS.GrupoPessoas, null, _gridProdutoEmbarcador);
    new BuscarProdutos(_regraICMS.AdicionarProdutoEmbarcador, undefined, undefined, undefined, undefined, undefined, undefined, _gridProdutoEmbarcador);

    _regraICMS.AdicionarProdutoEmbarcador.basicTable = _gridProdutoEmbarcador;
    _regraICMS.AdicionarProdutoEmbarcador.basicTable.CarregarGrid(new Array());
}

function ExcluirProdutoEmbarcadorClick(knoutProdutoEmbarcador, data) {
    var produtoEmbarcadorGrid = knoutProdutoEmbarcador.basicTable.BuscarRegistros();

    for (var i = 0; i < produtoEmbarcadorGrid.length; i++) {
        if (data.Codigo == produtoEmbarcadorGrid[i].Codigo) {
            produtoEmbarcadorGrid.splice(i, 1);
            break;
        }
    }

    knoutProdutoEmbarcador.basicTable.CarregarGrid(produtoEmbarcadorGrid);
}