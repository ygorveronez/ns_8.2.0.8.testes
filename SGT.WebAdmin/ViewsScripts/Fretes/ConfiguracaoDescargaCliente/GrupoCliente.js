var _gridGrupoCliente;

function GetSetGrupoCliente() {
    if (arguments.length == 0)
        return JSON.stringify(_gridGrupoCliente.BuscarRegistros().map(function (t) { return t.Codigo; }));

    var _dataGrupo = arguments[0];
    if (!$.isArray(_dataGrupo))
        _dataGrupo = JSON.parse(_dataGrupo);

    _gridGrupoCliente.CarregarGrid(_dataGrupo);
}

function CarregarGridGrupoCliente() {
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
                    RemoverGrupoClienteClick(data);
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
    _gridGrupoCliente = new BasicDataTable(_configuracaoDescargaCliente.GrupoClientes.idGrid, header, menuOpcoes, null, null, 10);
    _gridGrupoCliente.CarregarGrid([]);
}

function RemoverGrupoClienteClick(data) {
    var dataGrid = _gridGrupoCliente.BuscarRegistros();

    for (var i = 0; i < dataGrid.length; i++) {
        if (data.Codigo == dataGrid[i].Codigo) {
            dataGrid.splice(i, 1);
            break;
        }
    }

    _configuracaoDescargaCliente.GrupoClientes.val(dataGrid);
}