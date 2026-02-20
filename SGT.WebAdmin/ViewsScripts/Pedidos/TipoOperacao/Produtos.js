var _gridProdutos;

function buscarProdutosPadroes() {
    var excluir = {
        descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), evento: "onclick", metodo: function (data) { excluirProdutoPadrao(_gridProdutos, data) }, tamanho: "20", icone: ""
    };

    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [excluir], tamanho: 5 };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "40%" },
    ];
    _gridProdutos = new BasicDataTable(_tipoOperacao.Produtos.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    _gridProdutos.CarregarGrid([]);
    _tipoOperacao.Produtos.BasicDataTable = _gridProdutos;
}

function excluirProdutoPadrao(knout, data) {
    var produtoPadraoGrid = knout.BuscarRegistros();

    for (var i = 0; i < produtoPadraoGrid.length; i++) {
        if (data.Codigo == produtoPadraoGrid[i].Codigo) {
            produtoPadraoGrid.splice(i, 1);
            break;
        }
    }

    knout.CarregarGrid(produtoPadraoGrid);
}

function obterProdutoSalvar() {
    var listaProduto = _tipoOperacao.Produtos.BasicDataTable.BuscarRegistros();
    var listaProdutoRetornar = new Array();

    for (var i = 0; i < listaProduto.length; i++) {
        listaProdutoRetornar.push({
            Codigo: listaProduto[i].Codigo
        });
    }

    return JSON.stringify(listaProduto);
}