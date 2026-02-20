var _gridConfiguracoes;

function GetSetCliente() {
    if (arguments.length == 0)
        return JSON.stringify(_gridConfiguracoes.BuscarRegistros().map(function (t) { return t.Codigo; }));

    var _data = arguments[0];
    if (!$.isArray(_data))
        _data = JSON.parse(_data);

    _gridConfiguracoes.CarregarGrid(_data);
}

function CarregarGrid() {
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
                    RemoverClienteClick(data);
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
    _gridConfiguracoes = new BasicDataTable(_configuracaoDescargaCliente.Clientes.idGrid, header, menuOpcoes, null, null, 10);
    _gridConfiguracoes.CarregarGrid([]);
}

function RemoverClienteClick(data) {
    var dataGrid = _gridConfiguracoes.BuscarRegistros();

    for (var i = 0; i < dataGrid.length; i++) {
        if (data.Codigo == dataGrid[i].Codigo) {
            dataGrid.splice(i, 1);
            break;
        }
    }

    _configuracaoDescargaCliente.Clientes.val(dataGrid);
}