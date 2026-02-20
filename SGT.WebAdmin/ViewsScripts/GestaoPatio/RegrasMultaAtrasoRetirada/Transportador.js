/// <reference path="RegrasMultaAtrasoRetirada.js" />
/// <reference path="../../Consultas/Tranportador.js"/>

//*******MAPEAMENTO KNOUCKOUT*******

var RegrasMultaAtrasoRetiradaTransportadores;

//*******EVENTOS*******

function loadRegrasMultaAtrasoRetiradaTransportadores() {

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 15, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                excluirRegrasMultaAtrasoRetiradaTransportadores(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Transportador", width: "80%", className: "text-align-left" }
    ];

    RegrasMultaAtrasoRetiradaTransportadores = new BasicDataTable(_regrasMultaAtrasoRetirada.GridRegrasMultaAtrasoRetiradaTransportadores.id, header, menuOpcoes, { column: 2, dir: orderDir.asc });

    new BuscarTransportadores(_regrasMultaAtrasoRetirada.Transportadores, null, null, null, RegrasMultaAtrasoRetiradaTransportadores);
    _regrasMultaAtrasoRetirada.Transportadores.basicTable = RegrasMultaAtrasoRetiradaTransportadores;

    RecarregarGridRegrasMultaAtrasoRetiradaTransportadores();
}

function RecarregarGridRegrasMultaAtrasoRetiradaTransportadores() {
    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_regrasMultaAtrasoRetirada.RegrasMultaAtrasoRetiradaTransportadores.val())) {
        $.each(_regrasMultaAtrasoRetirada.RegrasMultaAtrasoRetiradaTransportadores.val(), function (i, Transportadores) {
            var obj = new Object();

            obj.Codigo = Transportadores.Codigo;
            obj.Descricao = Transportadores.Descricao;

            data.push(obj);
        });
    }
    RegrasMultaAtrasoRetiradaTransportadores.CarregarGrid(data);
}

function excluirRegrasMultaAtrasoRetiradaTransportadores(data) {
    var regrasMultaAtrasoRetiradaTransportadoresGrid = _regrasMultaAtrasoRetirada.Transportadores.basicTable.BuscarRegistros();

    for (var i = 0; i < regrasMultaAtrasoRetiradaTransportadoresGrid.length; i++) {
        if (data.Codigo == regrasMultaAtrasoRetiradaTransportadoresGrid[i].Codigo) {
            regrasMultaAtrasoRetiradaTransportadoresGrid.splice(i, 1);
            break;
        }
    }

    _regrasMultaAtrasoRetirada.Transportadores.basicTable.CarregarGrid(regrasMultaAtrasoRetiradaTransportadoresGrid);
}

function LimparRegrasMultaAtrasoRetiradaTransportadores() {
    _regrasMultaAtrasoRetirada.Transportadores.basicTable.CarregarGrid(new Array());
}