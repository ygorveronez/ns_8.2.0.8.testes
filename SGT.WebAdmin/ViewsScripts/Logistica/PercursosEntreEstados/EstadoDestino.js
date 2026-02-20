/// <reference path="../../Consultas/EstadoDestino.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="TabelaFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridEstadoDestino;
var _estadoDestino;

var EstadoDestino = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Tipo = PropertyEntity({ type: types.event, text: "Adicionar Estado de Destino", issue: 121, idBtnSearch: guid() });
}


//*******EVENTOS*******

function loadEstadoDestino() {

    _estadoDestino = new EstadoDestino();
    KoBindings(_estadoDestino, "knockoutEstadoDestino");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                excluirEstadoDestinoClick(_estadoDestino.Tipo, data)
            }
        }]
    };

    var header = [{ data: "Codigo", visible: false },
    { data: "Descricao", title: "Descrição", width: "80%" }];

    _gridEstadoDestino = new BasicDataTable(_estadoDestino.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarEstados(_estadoDestino.Tipo, callbackRetornoEstado);
    _estadoDestino.Tipo.basicTable = _gridEstadoDestino;
    //setarOrigemDestinoMapa();
    recarregarGridEstadoDestino();
}

function callbackRetornoEstado(dataRow) {
    var estadosDestino = new Array();
    $.each(_estadoDestino.Tipo.basicTable.BuscarRegistros(), function (i, estadoDestino) {
        estadosDestino.push(estadoDestino);
    });
    estadosDestino.push(dataRow);
    _gridEstadoDestino.CarregarGrid(estadosDestino);
    setarOrigemDestinoMapa();
}

function recarregarGridEstadoDestino() {
    var data = new Array();
    $.each(_percursosEntreEstados.EstadosDestino.val(), function (i, estadoDestino) {
        var estadoDestinoGrid = new Object();
        estadoDestinoGrid.Codigo = estadoDestino.Codigo;
        estadoDestinoGrid.Descricao = estadoDestino.Descricao;

        data.push(estadoDestinoGrid);
    });

    _gridEstadoDestino.CarregarGrid(data);
    setarOrigemDestinoMapa();

}


function excluirEstadoDestinoClick(knoutEstadoDestino, data) {
    var estadoDestinoGrid = knoutEstadoDestino.basicTable.BuscarRegistros();

    for (var i = 0; i < estadoDestinoGrid.length; i++) {
        if (data.Codigo == estadoDestinoGrid[i].Codigo) {
            estadoDestinoGrid.splice(i, 1);
            break;
        }
    }

    knoutEstadoDestino.basicTable.CarregarGrid(estadoDestinoGrid);
    setarOrigemDestinoMapa();
}

function limparCamposEstadoDestino() {
    LimparCampos(_estadoDestino);
}