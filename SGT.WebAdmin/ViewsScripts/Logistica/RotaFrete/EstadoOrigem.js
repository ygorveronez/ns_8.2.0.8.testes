/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
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

var _gridEstadoOrigem;
var _estadoOrigem;

var EstadoOrigem = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Estado = PropertyEntity({ type: types.event, text: Localization.Resources.Logistica.RotaFrete.AdicionarEstadoOrigem, idBtnSearch: guid(), issue: 12 });
}


//*******EVENTOS*******

function LoadEstadoOrigem() {

    _estadoOrigem = new EstadoOrigem();
    KoBindings(_estadoOrigem, "knockoutEstadoOrigem");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                ExcluirEstadoOrigemClick(_estadoOrigem.Estado, data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "80%" }
    ];

    _gridEstadoOrigem = new BasicDataTable(_estadoOrigem.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarEstados(_estadoOrigem.Estado, null, _gridEstadoOrigem, null);

    _estadoOrigem.Estado.basicTable = _gridEstadoOrigem;
    _estadoOrigem.Estado.basicTable.CarregarGrid(new Array());
}

function RecarregarGridEstadoOrigem() {
    _gridEstadoOrigem.CarregarGrid(_rotaFrete.EstadosOrigem.val());
}

function ExcluirEstadoOrigemClick(knoutEstadoOrigem, data) {

    var estadosGrid = knoutEstadoOrigem.basicTable.BuscarRegistros();

    for (var i = 0; i < estadosGrid.length; i++) {
        if (data.Codigo == estadosGrid[i].Codigo) {
            estadosGrid.splice(i, 1);
            break;
        }
    }

    knoutEstadoOrigem.basicTable.CarregarGrid(estadosGrid);
}

function LimparCamposEstadoOrigem() {
    LimparCampos(_estadoOrigem);
    _gridEstadoOrigem.CarregarGrid(new Array());
}