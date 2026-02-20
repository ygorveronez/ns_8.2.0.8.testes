var _gridTipoOperacao = null;

function LoadTiposOperacao() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirTipoOperacaoClick(_regraICMS.AdicionarTipoOperacao, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridTipoOperacao = new BasicDataTable(_regraICMS.GridTipoOperacao.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTiposOperacao(_regraICMS.AdicionarTipoOperacao, null, _regraICMS.GrupoPessoas, null, _gridTipoOperacao);

    _regraICMS.AdicionarTipoOperacao.basicTable = _gridTipoOperacao;
    _regraICMS.AdicionarTipoOperacao.basicTable.CarregarGrid(new Array());
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