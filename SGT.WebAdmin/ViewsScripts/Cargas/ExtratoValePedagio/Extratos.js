
var _extratos;
var _gridExtratos;
var _pesquisaExtratos;

var Extratos = function () {
    this.GridExtratos = PropertyEntity({ type: types.local, idGrid: guid() });
}

var PesquisaExtratos = function () {
    this.CodigoExtrato = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
}

function loadExtratos() {
    _extratos = new Extratos();
    KoBindings(_extratos, "knockoutExtratos");

    _pesquisaExtratos = new PesquisaExtratos();

    _gridExtratos = new GridView(_extratos.GridExtratos.idGrid, "ExtratoValePedagio/PesquisaExtratos", _pesquisaExtratos, null, null, 5);

    $("#modalExtratos")
        .on('hidden.bs.modal', function () { LimparCampos(_pesquisaExtratos); });
}

function consultarExtratosValePedagio() {
    Global.abrirModal("modalExtratos");
    _gridExtratos.CarregarGrid();
}