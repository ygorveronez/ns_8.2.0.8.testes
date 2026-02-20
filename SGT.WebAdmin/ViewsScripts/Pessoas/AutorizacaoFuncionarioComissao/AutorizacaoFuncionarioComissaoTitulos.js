/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _titulo;
var _gridTitulos;

var Titulos = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Titulos = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, idGrid: guid() });
}

//*******EVENTOS*******


function loadTitulos() {
    _titulo = new Titulos();
    KoBindings(_titulo, "knockoutTitulos");

    GridTitulos();
}

//*******MÉTODOS*******

function limparTitulos() {
    LimparCampos(_titulo);
    GridTitulos();
}

function GridTitulos() {
    _gridTitulos = new GridView(_titulo.Titulos.idGrid, "FuncionarioComissao/PesquisaTitulos", _titulo);
}

function CarregarTitulos(codigo) {
    _titulo.Codigo.val(codigo);
    _gridTitulos.CarregarGrid();
}