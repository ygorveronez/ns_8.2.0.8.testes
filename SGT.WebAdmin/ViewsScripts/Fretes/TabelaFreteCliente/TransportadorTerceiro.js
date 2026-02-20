var _gridTransportadorTerceiro = null;

function LoadTransportadoresTerceiros() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Fretes.TabelaFreteCliente.Excluir, id: guid(), metodo: function (data) {
                ExcluirTransportadorTerceiroClick(_tabelaFreteCliente.TransportadorTerceiro, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Fretes.TabelaFreteCliente.Descricao, width: "80%" }
    ];

    _gridTransportadorTerceiro = new BasicDataTable(_tabelaFreteCliente.GridTransportadorTerceiro.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarClientes(_tabelaFreteCliente.TransportadorTerceiro, null, false, [EnumModalidadePessoa.TransportadorTerceiro], null, _gridTransportadorTerceiro);

    _tabelaFreteCliente.TransportadorTerceiro.basicTable = _gridTransportadorTerceiro;
    _tabelaFreteCliente.TransportadorTerceiro.basicTable.CarregarGrid(new Array());
}

function ExcluirTransportadorTerceiroClick(knoutTransportadorTerceiro, data) {
    var transportadorTerceiroGrid = knoutTransportadorTerceiro.basicTable.BuscarRegistros();

    for (var i = 0; i < transportadorTerceiroGrid.length; i++) {
        if (data.Codigo == transportadorTerceiroGrid[i].Codigo) {
            transportadorTerceiroGrid.splice(i, 1);
            break;
        }
    }

    knoutTransportadorTerceiro.basicTable.CarregarGrid(transportadorTerceiroGrid);
}