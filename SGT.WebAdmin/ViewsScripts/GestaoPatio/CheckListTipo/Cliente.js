/// <reference path="../../Consultas/Cliente.js" />

var _gridCliente;

var Cliente = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Cliente = PropertyEntity({ type: types.event, text: "Adicionar Cliente", idBtnSearch: guid() });
}

function loadCliente() {
    _cliente = new Cliente();
    KoBindings(_cliente, "knockoutCliente");

    loadGridCliente();

    new BuscarClientes(_cliente.Cliente, function (r) {
        if (r != null) {
            var pAcessos = _gridCliente.BuscarRegistros();
            for (var i = 0; i < r.length; i++)
                pAcessos.push({
                    Codigo: r[i].Codigo,
                    Descricao: r[i].Descricao
                });

            _gridCliente.CarregarGrid(pAcessos);
            _checkListTipo.Clientes.multiplesEntities(_gridCliente.BuscarRegistros());
        }
    }, null, null, null, _gridCliente);

    _cliente.Cliente.basicTable = _gridCliente;
}

function loadGridCliente() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirCliente(_cliente.Cliente, data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "85%" }
    ];

    _gridCliente = new BasicDataTable(_cliente.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, 5);
    _gridCliente.CarregarGrid([]);
}

function ExcluirCliente(grid, registro) {
    let listaCliente = grid.basicTable.BuscarRegistros();
    for (let i = 0; i < listaCliente.length; i++) {
        if (listaCliente[i].Codigo == registro.Codigo) {
            listaCliente.splice(i, 1);
            break;
        }
    }
    grid.basicTable.CarregarGrid(listaCliente);
}
