/// <reference path="RegrasMultaAtrasoRetirada.js" />
/// <reference path="../../Consultas/Cliente.js"/>

//*******MAPEAMENTO KNOUCKOUT*******

var RegrasMultaAtrasoRetiradaClientes;

//*******EVENTOS*******

function loadRegrasMultaAtrasoRetiradaClientes() {

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 15, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                excluirRegrasMultaAtrasoRetiradaClientes(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Cliente", width: "80%", className: "text-align-left" }
    ];

    RegrasMultaAtrasoRetiradaClientes = new BasicDataTable(_regrasMultaAtrasoRetirada.GridRegrasMultaAtrasoRetiradaClientes.id, header, menuOpcoes, { column: 2, dir: orderDir.asc });

    new BuscarClientes(_regrasMultaAtrasoRetirada.Clientes, null, null, null, null, RegrasMultaAtrasoRetiradaClientes);
    _regrasMultaAtrasoRetirada.Clientes.basicTable = RegrasMultaAtrasoRetiradaClientes;

    RecarregarGridRegrasMultaAtrasoRetiradaClientes();
}

function RecarregarGridRegrasMultaAtrasoRetiradaClientes() {
    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_regrasMultaAtrasoRetirada.RegrasMultaAtrasoRetiradaClientes.val())) {
        $.each(_regrasMultaAtrasoRetirada.RegrasMultaAtrasoRetiradaClientes.val(), function (i, Clientes) {
            var obj = new Object();

            obj.Codigo = Clientes.Codigo;
            obj.Descricao = Clientes.Descricao;

            data.push(obj);
        });
    }

    RegrasMultaAtrasoRetiradaClientes.CarregarGrid(data);
}

function excluirRegrasMultaAtrasoRetiradaClientes(data) {
    var regrasMultaAtrasoRetiradaClientesGrid = _regrasMultaAtrasoRetirada.Clientes.basicTable.BuscarRegistros();

    for (var i = 0; i < regrasMultaAtrasoRetiradaClientesGrid.length; i++) {
        if (data.Codigo == regrasMultaAtrasoRetiradaClientesGrid[i].Codigo) {
            regrasMultaAtrasoRetiradaClientesGrid.splice(i, 1);
            break;
        }
    }

    _regrasMultaAtrasoRetirada.Clientes.basicTable.CarregarGrid(regrasMultaAtrasoRetiradaClientesGrid);
}

function LimparRegrasMultaAtrasoRetiradaClientes() {
    _regrasMultaAtrasoRetirada.Clientes.basicTable.CarregarGrid(new Array());
}