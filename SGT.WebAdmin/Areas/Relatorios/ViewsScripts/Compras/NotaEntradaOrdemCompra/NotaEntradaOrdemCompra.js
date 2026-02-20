//*******MAPEAMENTO KNOUCKOUT*******

var _gridNotaEntradaOrdemCompra, _pesquisaNotaEntradaOrdemCompra, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioNotaEntradaOrdemCompra;

var PesquisaNotaEntradaOrdemCompra = function () {
    this.Nota = PropertyEntity({ text: "Nota:", getType: typesKnockout.int });
    this.OrdemCompra = PropertyEntity({ text: "Ordem de Compra:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid() });
    this.Produto = PropertyEntity({ text: "Produto:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid() });
    this.Fornecedor = PropertyEntity({ text: "Fornecedor:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid() });
    this.DataEntrada = PropertyEntity({ text: "Data Entrada:", getType: typesKnockout.date });
    this.DataFinalEntrada = PropertyEntity({ text: "Data Final Entrada:", getType: typesKnockout.date });

    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridNotaEntradaOrdemCompra.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaNotaEntradaOrdemCompra.Visible.visibleFade()) {
                _pesquisaNotaEntradaOrdemCompra.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaNotaEntradaOrdemCompra.Visible.visibleFade(true);
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

function loadRelatorioNotaEntradaOrdemCompra() {
    _pesquisaNotaEntradaOrdemCompra = new PesquisaNotaEntradaOrdemCompra();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridNotaEntradaOrdemCompra = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/NotaEntradaOrdemCompra/Pesquisa", _pesquisaNotaEntradaOrdemCompra);

    _gridNotaEntradaOrdemCompra.SetPermitirEdicaoColunas(true);
    _gridNotaEntradaOrdemCompra.SetQuantidadeLinhasPorPagina(10);

    _relatorioNotaEntradaOrdemCompra = new RelatorioGlobal("Relatorios/NotaEntradaOrdemCompra/BuscarDadosRelatorio", _gridNotaEntradaOrdemCompra, function () {
        _relatorioNotaEntradaOrdemCompra.loadRelatorio(function () {
            KoBindings(_pesquisaNotaEntradaOrdemCompra, "knockoutPesquisaNotaEntradaOrdemCompra", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaNotaEntradaOrdemCompra", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaNotaEntradaOrdemCompra", false);

            new BuscarClientes(_pesquisaNotaEntradaOrdemCompra.Fornecedor);
            new BuscarProdutoTMS(_pesquisaNotaEntradaOrdemCompra.Produto);
            new BuscarOrdemCompra(_pesquisaNotaEntradaOrdemCompra.OrdemCompra);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaNotaEntradaOrdemCompra);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioNotaEntradaOrdemCompra.gerarRelatorio("Relatorios/NotaEntradaOrdemCompra/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioNotaEntradaOrdemCompra.gerarRelatorio("Relatorios/NotaEntradaOrdemCompra/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
