var _gridPesquisaGeracaoCargaEmbarcador;
var _pesquisaGeracaoCargaEmbarcador;

var PesquisaGeracaoCargaEmbarcador = function () {
    this.NumeroCarga = PropertyEntity({ text: "Nº da Carga: ", getType: typesKnockout.string, val: ko.observable(), visible: ko.observable(true) });
    this.NumeroCTe = PropertyEntity({ text: "Nº do CT-e: ", getType: typesKnockout.int, visible: ko.observable(true) });
    this.NumeroMDFe = PropertyEntity({ text: "Nº do MDF-e: ", getType: typesKnockout.int, visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridPesquisaGeracaoCargaEmbarcador.CarregarGrid();
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
};

//*******EVENTOS*******

function LoadGeracaoCargaEmbarcador() {
    _pesquisaGeracaoCargaEmbarcador = new PesquisaGeracaoCargaEmbarcador();
    KoBindings(_pesquisaGeracaoCargaEmbarcador, "knockoutPesquisaGeracaoCargaEmbarcador", false, _pesquisaGeracaoCargaEmbarcador.Pesquisar.id);

    LoadSelecaoMDFes();

    BuscarVeiculos(_pesquisaGeracaoCargaEmbarcador.Veiculo);
    BuscarMotoristas(_pesquisaGeracaoCargaEmbarcador.Motorista);

    BuscarGeracaoCargaEmbarcador();
}

//*******MÉTODOS*******

function BuscarGeracaoCargaEmbarcador() {
    _gridPesquisaGeracaoCargaEmbarcador = new GridView(_pesquisaGeracaoCargaEmbarcador.Pesquisar.idGrid, "GeracaoCargaEmbarcador/Pesquisa", _pesquisaGeracaoCargaEmbarcador, null, { column: 1, dir: orderDir.desc });
    _gridPesquisaGeracaoCargaEmbarcador.CarregarGrid();
}