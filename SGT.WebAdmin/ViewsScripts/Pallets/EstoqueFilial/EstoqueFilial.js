/// <reference path="../../Consultas/Filial.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridEstoqueFilial;
var _pesquisaEstoqueFilial;

var PesquisaEstoqueFilial = function () {
    
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFim = PropertyEntity({ text: "Data Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridEstoqueFilial.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.Filial.val.subscribe(function (novoValor) {
        if (novoValor == null || novoValor.trim() == "") {
            _pesquisaEstoqueFilial.Filial.codEntity(0);
            _gridEstoqueFilial.CarregarGrid();
        }
    });
}

//*******EVENTOS*******

function LoadEstoqueFilial() {

    _pesquisaEstoqueFilial = new PesquisaEstoqueFilial();
    KoBindings(_pesquisaEstoqueFilial, "knockoutPesquisaEstoqueFilial");

    new BuscarFilial(_pesquisaEstoqueFilial.Filial, function (r) {
        _pesquisaEstoqueFilial.Filial.val(r.Descricao);
        _pesquisaEstoqueFilial.Filial.codEntity(r.Codigo);
        _gridEstoqueFilial.CarregarGrid();
    });

    BuscarEstoqueFilial();
}

//*******MÉTODOS*******

function BuscarEstoqueFilial() {
    _gridEstoqueFilial = new GridView(_pesquisaEstoqueFilial.Pesquisar.idGrid, "EstoqueFilial/PesquisaExtrato", _pesquisaEstoqueFilial, null, { column: 1, dir: orderDir.desc }, 10);
    _gridEstoqueFilial.CarregarGrid();
}