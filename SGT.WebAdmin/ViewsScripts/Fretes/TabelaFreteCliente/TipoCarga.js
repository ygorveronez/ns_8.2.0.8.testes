var _gridTipoCarga = null;

function LoadTipoCarga() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Fretes.TabelaFreteCliente.Excluir, id: guid(), metodo: function (data) {
                ExcluirTipoCargaClick(_tabelaFreteCliente.TipoCarga, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Fretes.TabelaFreteCliente.Descricao, width: "80%" }
    ];

    _gridTipoCarga = new BasicDataTable(_tabelaFreteCliente.GridTipoCarga.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTiposdeCarga(_tabelaFreteCliente.TipoCarga, null, null, _gridTipoCarga);

    _tabelaFreteCliente.TipoCarga.basicTable = _gridTipoCarga;
    _tabelaFreteCliente.TipoCarga.basicTable.CarregarGrid(new Array());
}

function ExcluirTipoCargaClick(knoutTipoCarga, data) {
    var tipoCargaGrid = knoutTipoCarga.basicTable.BuscarRegistros();

    for (var i = 0; i < tipoCargaGrid.length; i++) {
        if (data.Codigo == tipoCargaGrid[i].Codigo) {
            tipoCargaGrid.splice(i, 1);
            break;
        }
    }

    knoutTipoCarga.basicTable.CarregarGrid(tipoCargaGrid);
}