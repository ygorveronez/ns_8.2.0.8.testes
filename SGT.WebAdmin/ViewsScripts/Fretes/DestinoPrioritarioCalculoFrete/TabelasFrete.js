var _gridTabelasFrete;

function GetSetTabelasFrete() {
    if (arguments.length == 0)
        return JSON.stringify(_gridTabelasFrete.BuscarRegistros().map(function (t) { return t.Codigo; }));

    var _data = arguments[0];
    if (!$.isArray(_data))
        _data = JSON.parse(_data);

    _gridTabelasFrete.CarregarGrid(_data);
}

function CarregarGridTabelaFrete() {
    //-- Grid
    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [
            {
                descricao: "Excluir",
                id: guid(),
                evento: "onclick",
                tamanho: "10",
                icone: "",
                metodo: function (data) {
                    RemoverTabelaFreteClick(data);
                }
            }
        ]
    };

    // Cabecalho
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "90%", className: "text-align-left" },
    ];

    // Grid
    _gridTabelasFrete = new BasicDataTable(_destinoPrioritarioCalculoFrete.TabelasFrete.idGrid, header, menuOpcoes, null, null, 10);
    _gridTabelasFrete.CarregarGrid([]);
}

function RemoverTabelaFreteClick(data) {
    var dataGrid = _destinoPrioritarioCalculoFrete.TabelasFrete.list;

    for (var i = 0; i < dataGrid.length; i++) {
        if (data.Codigo == dataGrid[i].Codigo) {
            dataGrid.splice(i, 1);
            break;
        }
    }
    _destinoPrioritarioCalculoFrete.TabelasFrete.list = dataGrid;
    _destinoPrioritarioCalculoFrete.TabelasFrete.val(dataGrid);
}