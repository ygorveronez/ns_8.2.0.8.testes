var _gridImportacaoHierarquia;
var _pesquisaImportacaoHierarquia;

var PesquisaImportacaoHierarquia = function () {
    this.DataInicial = PropertyEntity({ getType: typesKnockout.dateTime, val: ko.observable(""), text: "Data da Importação Inicial:" });
    this.DataFinal = PropertyEntity({ getType: typesKnockout.dateTime, val: ko.observable(""), text: "Data da Importação Final:" });

    this.DataFinal.dateRangeInit = this.DataInicial;
    this.DataInicial.dateRangeLimit = this.DataFinal;

    this.Pesquisar = PropertyEntity({
        eventClick: function () {
            _gridImportacaoHierarquia.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

function loadImportacaoHierarquia() {
    _pesquisaImportacaoHierarquia = new PesquisaImportacaoHierarquia();
    KoBindings(_pesquisaImportacaoHierarquia, "knockoutPesquisaImportacaoHierarquia");

    loadGridImportacaoHierarquia();
}

function loadGridImportacaoHierarquia() {
    _gridImportacaoHierarquia = new GridView(_pesquisaImportacaoHierarquia.Pesquisar.idGrid, "ImportacaoHierarquia/Pesquisa", _pesquisaImportacaoHierarquia, null, null, 20, null, null, null, null, 60);
    _gridImportacaoHierarquia.CarregarGrid();
}