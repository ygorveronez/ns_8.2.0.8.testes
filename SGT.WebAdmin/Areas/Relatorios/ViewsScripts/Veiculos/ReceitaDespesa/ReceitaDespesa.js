//*******MAPEAMENTO KNOUCKOUT*******

var _gridVeiculoReceitaDespesa, _pesquisaVeiculoReceitaDespesa, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioVeiculoReceitaDespesa;

var PesquisaVeiculoReceitaDespesa = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataInicial = PropertyEntity({ getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), text: "Data Inicial:" });
    this.DataFinal = PropertyEntity({ getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), text: "Data Final:" });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.ModeloVeicular = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo Veicular:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.SegmentoVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Segmento:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridVeiculoReceitaDespesa.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaVeiculoReceitaDespesa.Visible.visibleFade() == true) {
                _pesquisaVeiculoReceitaDespesa.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaVeiculoReceitaDespesa.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function LoadVeiculoReceitaDespesa() {
    _pesquisaVeiculoReceitaDespesa = new PesquisaVeiculoReceitaDespesa();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridVeiculoReceitaDespesa = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ReceitaDespesa/Pesquisa", _pesquisaVeiculoReceitaDespesa);

    _gridVeiculoReceitaDespesa.SetPermitirEdicaoColunas(true);
    _gridVeiculoReceitaDespesa.SetQuantidadeLinhasPorPagina(10);

    _relatorioVeiculoReceitaDespesa = new RelatorioGlobal("Relatorios/ReceitaDespesa/BuscarDadosRelatorio", _gridVeiculoReceitaDespesa, function () {
        _relatorioVeiculoReceitaDespesa.loadRelatorio(function () {
            KoBindings(_pesquisaVeiculoReceitaDespesa, "knockoutPesquisaVeiculoReceitaDespesa", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaVeiculoReceitaDespesa", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaVeiculoReceitaDespesa", false);

            new BuscarVeiculos(_pesquisaVeiculoReceitaDespesa.Veiculo);
            new BuscarSegmentoVeiculo(_pesquisaVeiculoReceitaDespesa.SegmentoVeiculo);
            new BuscarModelosVeicularesCarga(_pesquisaVeiculoReceitaDespesa.ModeloVeicular);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaVeiculoReceitaDespesa);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioVeiculoReceitaDespesa.gerarRelatorio("Relatorios/ReceitaDespesa/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioVeiculoReceitaDespesa.gerarRelatorio("Relatorios/ReceitaDespesa/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
