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
/// <reference path="Bem.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _bemFinanciamento;
var _gridBemFinanciamento;

var BemFinanciamento = function () {
    this.Grid = PropertyEntity({ type: types.local });
}

//*******EVENTOS*******

function loadBemFinanciamento() {
    _bemFinanciamento = new BemFinanciamento();
    KoBindings(_bemFinanciamento, "knockoutFinanciamentoBem");

    _gridBemFinanciamento = new GridView(_bemFinanciamento.Grid.id, "Bem/FinanciamentoBem", _bem);
}

//*******MÉTODOS*******

function buscarBemFinanciamento() {
    _gridBemFinanciamento.CarregarGrid();
}