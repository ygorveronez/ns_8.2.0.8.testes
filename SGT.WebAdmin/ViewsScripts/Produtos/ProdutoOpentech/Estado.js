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
/// <reference path="../../Consultas/Estado.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridEstado;
var _estado;

var Estado = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Estado = PropertyEntity({ type: types.event, text: "Adicionar Estado", idBtnSearch: guid(), issue: 12 });
}


//*******EVENTOS*******

function LoadEstado() {

    _estado = new Estado();
    KoBindings(_estado, "knockoutEstado");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirEstadoClick(_estado.Estado, data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridEstado = new BasicDataTable(_estado.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarEstados(_estado.Estado, null, _gridEstado, function () {
        _produtoOpentech.Estados.val(_estado.Estado.basicTable.BuscarRegistros());

    });

    _estado.Estado.basicTable = _gridEstado;
    _estado.Estado.basicTable.CarregarGrid(new Array());
}

function RecarregarGridEstado() {
    _gridEstado.CarregarGrid(_produtoOpentech.Estados.val());
}

function ExcluirEstadoClick(knoutEstado, data) {
    var estadosGrid = knoutEstado.basicTable.BuscarRegistros();
    var localidadesGrid = _gridLocalidade.BuscarRegistros();

    for (var i = 0; i < localidadesGrid.length; i++) {
        if (data.Codigo == localidadesGrid[i].Estado) {
            localidadesGrid.splice(i, 1);
            break;
        }
    }

    for (var i = 0; i < estadosGrid.length; i++) {
        if (data.Codigo == estadosGrid[i].Codigo) {
            estadosGrid.splice(i, 1);
            break;
        }
    }

    _gridLocalidade.CarregarGrid(localidadesGrid);
    knoutEstado.basicTable.CarregarGrid(estadosGrid);
}

function LimparCamposEstado() {
    LimparCampos(_estado);
    _gridEstado.CarregarGrid(new Array());
}