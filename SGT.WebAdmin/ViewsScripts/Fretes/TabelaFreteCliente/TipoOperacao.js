var _gridTipoOperacao = null;

function LoadTiposOperacao() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Fretes.TabelaFreteCliente.Excluir, id: guid(), metodo: function (data) {
                ExcluirTipoOperacaoClick(_tabelaFreteCliente.TipoOperacao, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Fretes.TabelaFreteCliente.Descricao, width: "80%" }
    ];

    _gridTipoOperacao = new BasicDataTable(_tabelaFreteCliente.GridTipoOperacao.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTiposOperacao(_tabelaFreteCliente.TipoOperacao, null, _tabelaFreteCliente.GrupoPessoas, null, _gridTipoOperacao);

    _tabelaFreteCliente.TipoOperacao.basicTable = _gridTipoOperacao;
    _tabelaFreteCliente.TipoOperacao.basicTable.CarregarGrid(new Array());
}

function ExcluirTipoOperacaoClick(knoutTipoOperacao, data) {
    var tipoOperacaoGrid = knoutTipoOperacao.basicTable.BuscarRegistros();

    for (var i = 0; i < tipoOperacaoGrid.length; i++) {
        if (data.Codigo == tipoOperacaoGrid[i].Codigo) {
            tipoOperacaoGrid.splice(i, 1);
            break;
        }
    }

    knoutTipoOperacao.basicTable.CarregarGrid(tipoOperacaoGrid);
}