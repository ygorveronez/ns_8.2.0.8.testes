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
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="ContratoNotaFiscal.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTransportadorContratoNotaFiscal;
var _transportadorContratoNotaFiscal;

var TransportadorContratoNotaFiscal = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Transportador = PropertyEntity({ type: types.event, text: "Adicionar Transportador", idBtnSearch: guid() });
};

//*******EVENTOS*******

function LoadTransportadorContratoNotaFiscal() {
    _transportadorContratoNotaFiscal = new TransportadorContratoNotaFiscal();
    KoBindings(_transportadorContratoNotaFiscal, "knockoutContratoTransportador");

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        $("#liTabTransportadores").show();
    }

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [{ descricao: "Excluir", id: guid(), metodo: function (data) { ExcluirTransportadorContratoNotaFiscalClick(data) } }] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridTransportadorContratoNotaFiscal = new BasicDataTable(_transportadorContratoNotaFiscal.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTransportadores(_transportadorContratoNotaFiscal.Transportador, null, null, null, _gridTransportadorContratoNotaFiscal);
    _transportadorContratoNotaFiscal.Transportador.basicTable = _gridTransportadorContratoNotaFiscal;
    _gridTransportadorContratoNotaFiscal.CarregarGrid([]);
}

function ExcluirTransportadorContratoNotaFiscalClick(data) {
    var transportadorContratoNotaFiscalGrid = _transportadorContratoNotaFiscal.Transportador.basicTable.BuscarRegistros();

    for (var i = 0; i < transportadorContratoNotaFiscalGrid.length; i++) {
        if (data.Codigo == transportadorContratoNotaFiscalGrid[i].Codigo) {
            transportadorContratoNotaFiscalGrid.splice(i, 1);
            break;
        }
    }

    _transportadorContratoNotaFiscal.Transportador.basicTable.CarregarGrid(transportadorContratoNotaFiscalGrid);
}

function ObterTransportadorContratoNotaFiscalSalvar() {
    var transportadorContratoNotaFiscal = _transportadorContratoNotaFiscal.Transportador.basicTable.BuscarRegistros();
    var transportadorContratoNotaFiscalRetornar = new Array();

    for (var i = 0; i < transportadorContratoNotaFiscal.length; i++) {
        transportadorContratoNotaFiscalRetornar.push({
            Codigo: transportadorContratoNotaFiscal[i].Codigo
        });
    }

    return JSON.stringify(transportadorContratoNotaFiscalRetornar);
}

function preencherListaTransportadorContratoNotaFiscal(data) {
    _gridTransportadorContratoNotaFiscal.CarregarGrid(data.Transportadores);
}

function LimparCamposTransportadorContratoNotaFiscal() {
    _gridTransportadorContratoNotaFiscal.CarregarGrid([]);
}