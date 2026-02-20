/// <reference path="RegrasMultaAtrasoRetirada.js" />
/// <reference path="../../Consultas/Localidade.js"/>

//*******MAPEAMENTO KNOUCKOUT*******

var RegrasMultaAtrasoRetiradaLocalidades;

//*******EVENTOS*******

function loadRegrasMultaAtrasoRetiradaLocalidades() {

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 15, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                excluirRegrasMultaAtrasoRetiradaLocalidades(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Cidade", width: "80%", className: "text-align-left" }
    ];

    RegrasMultaAtrasoRetiradaLocalidades = new BasicDataTable(_regrasMultaAtrasoRetirada.GridRegrasMultaAtrasoRetiradaLocalidades.id, header, menuOpcoes, { column: 2, dir: orderDir.asc });

    new BuscarLocalidades(_regrasMultaAtrasoRetirada.Localidades, null, null, null, RegrasMultaAtrasoRetiradaLocalidades);
    _regrasMultaAtrasoRetirada.Localidades.basicTable = RegrasMultaAtrasoRetiradaLocalidades;

    RecarregarGridRegrasMultaAtrasoRetiradaLocalidades();
}

function RecarregarGridRegrasMultaAtrasoRetiradaLocalidades() {
    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_regrasMultaAtrasoRetirada.RegrasMultaAtrasoRetiradaLocalidades.val())) {
        $.each(_regrasMultaAtrasoRetirada.RegrasMultaAtrasoRetiradaLocalidades.val(), function (i, Localidades) {
            var obj = new Object();

            obj.Codigo = Localidades.Codigo;
            obj.Descricao = Localidades.Descricao;

            data.push(obj);
        });
    }
    RegrasMultaAtrasoRetiradaLocalidades.CarregarGrid(data);
}

function excluirRegrasMultaAtrasoRetiradaLocalidades(data) {
    var regrasMultaAtrasoRetiradaLocalidadesGrid = _regrasMultaAtrasoRetirada.Localidades.basicTable.BuscarRegistros();

    for (var i = 0; i < regrasMultaAtrasoRetiradaLocalidadesGrid.length; i++) {
        if (data.Codigo == regrasMultaAtrasoRetiradaLocalidadesGrid[i].Codigo) {
            regrasMultaAtrasoRetiradaLocalidadesGrid.splice(i, 1);
            break;
        }
    }

    _regrasMultaAtrasoRetirada.Localidades.basicTable.CarregarGrid(regrasMultaAtrasoRetiradaLocalidadesGrid);
}

function LimparRegrasMultaAtrasoRetiradaLocalidades() {
    _regrasMultaAtrasoRetirada.Localidades.basicTable.CarregarGrid(new Array());
}