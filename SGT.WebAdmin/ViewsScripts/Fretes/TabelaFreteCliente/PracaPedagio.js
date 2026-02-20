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
/// <reference path="../../Consultas/RotaFrete.js" />
/// <reference path="TabelaFreteCliente.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pracaPedagio, _gridPracaPedagio;

var PracaPedagio = function () {
    this.Grid = PropertyEntity({ type: types.local, idGrid: guid(), });
    this.RotaFrete = _tabelaFreteCliente.RotaFrete;
    this.RotaFrete.val.subscribe(function (value) {
        recarregarGridPracaPedagioTarifaRota();
    });
}

//*******EVENTOS*******

function loadPracaPedagio() {
    _pracaPedagio = new PracaPedagio();
    KoBindings(_pracaPedagio, "knockoutPracaPedagio");

    new BuscarRotasFrete(_pracaPedagio.RotaFrete);
}

function limparCamposPracaPedagio() {
    LimparCampos(_pracaPedagio);
}

function recarregarGridPracaPedagioTarifaRota() {
    if (_pracaPedagio.RotaFrete.codEntity() > 0) {
        $("#grid-tabela-frete-cliente-praca-pedagio-tarifa").show();
        _gridPracaPedagio = new GridView(_pracaPedagio.Grid.idGrid, "TabelaFreteCliente/BuscarPracaPedagioTarifaRota?Codigo=" + _pracaPedagio.RotaFrete.codEntity(), null, null, null, 10, null, true, null, null, 1000, true, null, null, true, null, false);
        _gridPracaPedagio.CarregarGrid();
    } else {
        $("#grid-tabela-frete-cliente-praca-pedagio-tarifa").hide();
    }
}