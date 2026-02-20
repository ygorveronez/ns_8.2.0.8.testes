/// <reference path="ContratoFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _acrescimoDesconto;
var _gridAcrescimoDesconto;

var AcrescimoDesconto = function () {
    this.GridAcrescimoDesconto = PropertyEntity({ idGrid: guid() });
};

//*******EVENTOS*******

function LoadAcrescimoDesconto() {
    _acrescimoDesconto = new AcrescimoDesconto();
    KoBindings(_acrescimoDesconto, "knoutAcrescimoDesconto");
}

function BuscarAcrescimosDescontos() {
    _gridAcrescimoDesconto = new GridView(_acrescimoDesconto.GridAcrescimoDesconto.idGrid, "ContratoFrete/PesquisaAcrescimoDesconto", _contratoFrete);
    _gridAcrescimoDesconto.onBeforeGridLoad(VisibilidadeAbaAcrescimoDesconto);
    _gridAcrescimoDesconto.CarregarGrid();
}

function VisibilidadeAbaAcrescimoDesconto(data) {
    $("#liTabAcrescimoDesconto").hide();
    if (data.recordsTotal > 0)
        $("#liTabAcrescimoDesconto").show();
}