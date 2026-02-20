/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="Transportador.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridOperadores;
var _operador;

var Operador = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Operadores = PropertyEntity({ type: types.event, text: "Adicionar Operadores", idBtnSearch: guid() });
}

//*******EVENTOS*******

function loadOperadores() {

    _operador = new Operador();
    KoBindings(_operador, "knockoutOperador");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                excluirOperadoresClick(_operador.Operadores, data)
            }
        }]
    };

    var header = [{ data: "Codigo", visible: false },
    { data: "Descricao", title: "Descrição", width: "80%" }];

    _gridOperadores = new BasicDataTable(_operador.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarFuncionario(_operador.Operadores, null, _gridOperadores);
    _operador.Operadores.basicTable = _gridOperadores;

    recarregarGridOperadores();
}

function recarregarGridOperadores() {

    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_transportador.Operadores.val())) {
        $.each(_transportador.Operadores.val(), function (i, operador) {
            var operadoresGrid = new Object();
            operadoresGrid.Codigo = operador.Tipo.Codigo;
            operadoresGrid.Descricao = operador.Tipo.Descricao;

            data.push(operadoresGrid);
        });
    }

    _gridOperadores.CarregarGrid(data);
}


function excluirOperadoresClick(knoutOperadores, data) {
    var operadoresGrid = knoutOperadores.basicTable.BuscarRegistros();

    for (var i = 0; i < operadoresGrid.length; i++) {
        if (data.Codigo == operadoresGrid[i].Codigo) {
            operadoresGrid.splice(i, 1);
            break;
        }
    }

    knoutOperadores.basicTable.CarregarGrid(operadoresGrid);
}

function limparCamposOperadores() {
    LimparCampos(_operador);
    LimparCampos(_gridOperadores);
}