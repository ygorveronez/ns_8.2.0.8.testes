/// <reference path="../../Consultas/TipoOperacao.js" />
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

var _gridTransportador;
var _transportador;

var Transportador = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Transportador = PropertyEntity({ type: types.event, text: Localization.Resources.Fretes.TabelaFrete.AdicionarTransportador, idBtnSearch: guid() });
}


//*******EVENTOS*******

function loadTransportador() {

    _transportador = new Transportador();
    KoBindings(_transportador, "knockoutTransportador");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Fretes.TabelaFrete.Excluir, id: guid(), metodo: function (data) {
                excluirTransportadorClick(_transportador.Transportador, data)
            }
        }]
    };

    var header = [{ data: "Codigo", visible: false },
    { data: "Descricao", title: Localization.Resources.Fretes.TabelaFrete.Descricao, width: "80%" }];

    _gridTransportador = new BasicDataTable(_transportador.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTransportadores(_transportador.Transportador, null, null, null, _gridTransportador);
    _transportador.Transportador.basicTable = _gridTransportador;

    recarregarGridTransportador();
}

function recarregarGridTransportador() {
    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_tabelaFrete.Transportadores.val())) {

        $.each(_tabelaFrete.Transportadores.val(), function (i, transportador) {
            var transportadorGrid = new Object();

            transportadorGrid.Codigo = transportador.Transportador.Codigo;
            transportadorGrid.Descricao = transportador.Transportador.Descricao;

            data.push(transportadorGrid);
        });
    }

    _gridTransportador.CarregarGrid(data);
}


function excluirTransportadorClick(knoutTransportador, data) {
    var transportadorGrid = knoutTransportador.basicTable.BuscarRegistros();

    for (var i = 0; i < transportadorGrid.length; i++) {
        if (data.Codigo == transportadorGrid[i].Codigo) {
            transportadorGrid.splice(i, 1);
            break;
        }
    }

    knoutTransportador.basicTable.CarregarGrid(transportadorGrid);
}

function limparCamposTransportador() {
    LimparCampos(_transportador);
}

function obterGridTransportadores() {
    return _gridTransportador;
}