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

var _gridEstadoDestino;
var _estadoDestino;

var EstadoDestino = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Estado = PropertyEntity({ type: types.event, text: Localization.Resources.Logistica.RotaFrete.AdicionarEstadoDestino, idBtnSearch: guid(), issue: 12 });
};

//*******EVENTOS*******

function LoadEstadoDestino() {

    _estadoDestino = new EstadoDestino();
    KoBindings(_estadoDestino, "knockoutEstadoDestino");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                ExcluirEstadoDestinoClick(_estadoDestino.Estado, data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "80%" }
    ];

    _gridEstadoDestino = new BasicDataTable(_estadoDestino.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarEstados(_estadoDestino.Estado, null, _gridEstadoDestino, VerificarVisibilidadeCamposPorEstadoDestino);

    _estadoDestino.Estado.basicTable = _gridEstadoDestino;
    _estadoDestino.Estado.basicTable.CarregarGrid(new Array());
}

function VerificarVisibilidadeCamposPorEstadoDestino() {
    if (_estadoDestino.Estado.basicTable.BuscarRegistros().length > 0) {
        _rotaFrete.TipoUltimoPontoRoteirizacaoPorEstado.visible(true);
        ControleVisibilidadeAbaRoteirizacao();
    } else {
        _rotaFrete.TipoUltimoPontoRoteirizacaoPorEstado.visible(false);
        LimparCampo(_rotaFrete.TipoUltimoPontoRoteirizacaoPorEstado);
        ControleVisibilidadeAbaRoteirizacao();
    }
}

function RecarregarGridEstadoDestino() {
    _gridEstadoDestino.CarregarGrid(_rotaFrete.Estados.val());

    VerificarVisibilidadeCamposPorEstadoDestino();
}

function ExcluirEstadoDestinoClick(knoutEstado, data) {

    var estadosGrid = knoutEstado.basicTable.BuscarRegistros();

    for (var i = 0; i < estadosGrid.length; i++) {
        if (data.Codigo == estadosGrid[i].Codigo) {
            estadosGrid.splice(i, 1);
            break;
        }
    }

    knoutEstado.basicTable.CarregarGrid(estadosGrid);

    VerificarVisibilidadeCamposPorEstadoDestino();
}

function LimparCamposEstadoDestino() {
    LimparCampos(_estadoDestino);
    _gridEstadoDestino.CarregarGrid(new Array());

    VerificarVisibilidadeCamposPorEstadoDestino();
}