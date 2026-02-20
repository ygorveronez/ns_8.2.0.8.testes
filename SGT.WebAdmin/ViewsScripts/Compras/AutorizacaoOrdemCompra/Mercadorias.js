/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="AutorizacaoOrdemCompra.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _mercadoriaAutorizacaoOrdemCompra;
var _gridMercadoriasAutorizacaoOrdemCompra;

var MercadoriasAutorizacaoOrdemCompra = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Mercadorias = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, idGrid: guid() });
};

//*******EVENTOS*******

function loadMercadoriasAutorizacaoOrdemCompra() {
    _mercadoriaAutorizacaoOrdemCompra = new MercadoriasAutorizacaoOrdemCompra();
    KoBindings(_mercadoriaAutorizacaoOrdemCompra, "knockoutMercadoriasAutorizacaoOrdemCompra");

    GridMercadoriasAutorizacaoOrdemCompra();
}

//*******MÉTODOS*******

function limparMercadoriasAutorizacaoOrdemCompra() {
    LimparCampos(_mercadoriaAutorizacaoOrdemCompra);
    GridMercadoriasAutorizacaoOrdemCompra();
}

function GridMercadoriasAutorizacaoOrdemCompra() {
    _gridMercadoriasAutorizacaoOrdemCompra = new GridView(_mercadoriaAutorizacaoOrdemCompra.Mercadorias.idGrid, "OrdemCompra/PesquisaMercadorias", _mercadoriaAutorizacaoOrdemCompra);
}

function CarregarMercadoriasAutorizacaoOrdemCompra(codigo) {
    _mercadoriaAutorizacaoOrdemCompra.Codigo.val(codigo);
    _gridMercadoriasAutorizacaoOrdemCompra.CarregarGrid();
}