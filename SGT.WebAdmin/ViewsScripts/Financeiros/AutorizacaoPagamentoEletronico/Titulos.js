/// <reference path="AutorizacaoPagamentoEletronico.js" />
/// <reference path="AutorizarRegras.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _titulos;
var _gridTitulos;

/*
 * Declaração das Classes
 */

var Titulos = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Titulos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid() });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridTitulos() {
    _titulos.Codigo.val(_pagamentoEletronico.Codigo.val());
    _titulos.Usuario.val(_pagamentoEletronico.Usuario.val());

    _gridTitulos = new GridView(_titulos.Titulos.idGrid, "AutorizacaoPagamentoEletronico/Titulos", _titulos);
}

function loadTitulos() {
    _titulos = new Titulos();
    KoBindings(_titulos, "knockoutTitulos");

    loadGridTitulos();
}

/*
 * Declaração das Funções
 */

function AtualizarGridTitulos() {
    _titulos.Codigo.val(_pagamentoEletronico.Codigo.val());
    _titulos.Usuario.val(_pagamentoEletronico.Usuario.val());

    _gridTitulos.CarregarGrid();
}

function limparTitulos() {
    //_gridTitulos.CarregarGrid();
}
