/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />>

//*******MAPEAMENTO KNOUCKOUT*******

var _mercadoriaAutorizacao;
var _gridMercadoriasAutorizacao;

var MercadoriasAutorizacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Mercadorias = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, idGrid: guid() });
};

//*******EVENTOS*******

function loadMercadoriasAutorizacao() {
    _mercadoriaAutorizacao = new MercadoriasAutorizacao();
    KoBindings(_mercadoriaAutorizacao, "knockoutMercadoriasAutorizacao");

    GridMercadoriasAutorizacao();
}

//*******MÉTODOS*******

function limparMercadorias() {
    LimparCampos(_mercadoriaAutorizacao);
    GridMercadoriasAutorizacao();
}

function GridMercadoriasAutorizacao() {
    _gridMercadoriasAutorizacao = new GridView(_mercadoriaAutorizacao.Mercadorias.idGrid, "RequisicaoMercadoria/PesquisaMercadorias", _mercadoriaAutorizacao);
}

function CarregarMercadorias(codigo) {
    _mercadoriaAutorizacao.Codigo.val(codigo);
    _gridMercadoriasAutorizacao.CarregarGrid();
}