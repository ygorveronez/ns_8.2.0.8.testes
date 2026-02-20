/// <reference path="RegrasMultaAtrasoRetirada.js" />
/// <reference path="../../Consultas/Estado.js"/>

//*******MAPEAMENTO KNOUCKOUT*******

var RegrasMultaAtrasoRetiradaEstados;

//*******EVENTOS*******

function loadGridRegrasMultaAtrasoRetiradaEstados() {

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 15, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                excluirRegrasMultaAtrasoRetiradaEstados(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Estado", width: "80%", className: "text-align-left" }
    ];

    RegrasMultaAtrasoRetiradaEstados = new BasicDataTable(_regrasMultaAtrasoRetirada.GridRegrasMultaAtrasoRetiradaEstados.id, header, menuOpcoes, { column: 2, dir: orderDir.asc });

    new BuscarEstados(_regrasMultaAtrasoRetirada.Estados, null, RegrasMultaAtrasoRetiradaEstados);
    _regrasMultaAtrasoRetirada.Estados.basicTable = RegrasMultaAtrasoRetiradaEstados;

    RecarregarGridRegrasMultaAtrasoRetiradaEstados();
}

function RecarregarGridRegrasMultaAtrasoRetiradaEstados() {
    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_regrasMultaAtrasoRetirada.RegrasMultaAtrasoRetiradaEstados.val())) {
        $.each(_regrasMultaAtrasoRetirada.RegrasMultaAtrasoRetiradaEstados.val(), function (i, Estados) {
            var obj = new Object();

            obj.Codigo = Estados.Codigo;
            obj.Descricao = Estados.Descricao;

            data.push(obj);
        });
    }
    RegrasMultaAtrasoRetiradaEstados.CarregarGrid(data);
}

function excluirRegrasMultaAtrasoRetiradaEstados(data) {
    var regrasMultaAtrasoRetiradaEstadosGrid = _regrasMultaAtrasoRetirada.Estados.basicTable.BuscarRegistros();

    for (var i = 0; i < regrasMultaAtrasoRetiradaEstadosGrid.length; i++) {
        if (data.Codigo == regrasMultaAtrasoRetiradaEstadosGrid[i].Codigo) {
            regrasMultaAtrasoRetiradaEstadosGrid.splice(i, 1);
            break;
        }
    }

    _regrasMultaAtrasoRetirada.Estados.basicTable.CarregarGrid(regrasMultaAtrasoRetiradaEstadosGrid);
}

function LimparRegrasMultaAtrasoRetiradaEstados() {
    _regrasMultaAtrasoRetirada.Estados.basicTable.CarregarGrid(new Array());
}