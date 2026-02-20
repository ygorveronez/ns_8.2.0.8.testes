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
/// <reference path="ConfigOperador.js" />
/// <reference path="../../Consultas/Tranportador.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTransportador;
var _transportador;

var Transportador = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Transportador = PropertyEntity({ type: types.event, text: Localization.Resources.Operacional.ConfigOperador.AdicionarTransportador, idBtnSearch: guid() });
};

//*******EVENTOS*******

function LoadTransportador() {
    _transportador = new Transportador();
    KoBindings(_transportador, "knockoutOperadorTransportador");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Operacional.ConfigOperador.Excluir, id: guid(), metodo: function (data) {
                ExcluirTransportadorClick(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Operacional.ConfigOperador.Descricao, width: "80%" }
    ];

    _gridTransportador = new BasicDataTable(_transportador.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTransportadores(_transportador.Transportador, null, null, null, _gridTransportador, null, null, null, null, false);
    _transportador.Transportador.basicTable = _gridTransportador;

    RecarregarGridTransportador();
}

function RecarregarGridTransportador() {

    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_operador.Transportadores.val())) {
        $.each(_operador.Transportadores.val(), function (i, transportador) {
            var transportadorGrid = new Object();

            transportadorGrid.Codigo = transportador.Codigo;
            transportadorGrid.Descricao = transportador.Descricao;

            data.push(transportadorGrid);
        });
    }

    _gridTransportador.CarregarGrid(data);
}

function ExcluirTransportadorClick(data) {
    var transportadorGrid = _transportador.Transportador.basicTable.BuscarRegistros();

    for (var i = 0; i < transportadorGrid.length; i++) {
        if (data.Codigo == transportadorGrid[i].Codigo) {
            transportadorGrid.splice(i, 1);
            break;
        }
    }

    _transportador.Transportador.basicTable.CarregarGrid(transportadorGrid);
}

function LimparCamposTransportador() {
    LimparCampos(_transportador);
    _transportador.Transportador.basicTable.CarregarGrid(new Array());
}