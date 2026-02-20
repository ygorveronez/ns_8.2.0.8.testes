var _gridFronteira = null;

function LoadFronteira() {
    let menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Fretes.TabelaFreteCliente.Excluir, id: guid(), metodo: function (data) {
                ExcluirFronteiraClick(_tabelaFreteCliente.Fronteira, data);
            }
        }]
    };

    let header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Fretes.TabelaFreteCliente.Descricao, width: "80%" }
    ];

    _gridFronteira = new BasicDataTable(_tabelaFreteCliente.GridFronteira.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    BuscarClientes(_tabelaFreteCliente.Fronteira, null, null, null, null, _gridFronteira, null, null, null, null, null, null, null, null, null, null, true, null);

    _tabelaFreteCliente.Fronteira.basicTable = _gridFronteira;
    _tabelaFreteCliente.Fronteira.basicTable.CarregarGrid(new Array());
}

function ExcluirFronteiraClick(knoutFronteira, data) {
    let fronteiraGrid = knoutFronteira.basicTable.BuscarRegistros();

    for (var i = 0; i < fronteiraGrid.length; i++) {
        if (data.Codigo == fronteiraGrid[i].Codigo) {
            fronteiraGrid.splice(i, 1);
            break;
        }
    }

    knoutFronteira.basicTable.CarregarGrid(fronteiraGrid);
}