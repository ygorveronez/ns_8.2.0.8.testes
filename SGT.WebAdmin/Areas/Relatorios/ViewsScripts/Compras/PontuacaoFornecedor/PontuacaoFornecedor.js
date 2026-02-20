//*******MAPEAMENTO KNOUCKOUT*******

var _gridPontuacaoFornecedor, _pesquisaPontuacaoFornecedor, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioPontuacaoFornecedor;


var PesquisaPontuacaoFornecedor = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date });
    this.Comprador = PropertyEntity({ text: "Comprador:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid() });
    this.Fornecedor = PropertyEntity({ text: "Fornecedor:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid() });
    this.OrdemCompra = PropertyEntity({ text: "Ordem de Compra:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid() });
    this.Produto = PropertyEntity({ text: "Produto:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid() });
    this.NotaEntrada = PropertyEntity({ text: "Nota de Entrada:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid() });

    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridPontuacaoFornecedor.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaPontuacaoFornecedor.Visible.visibleFade() == true) {
                _pesquisaPontuacaoFornecedor.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaPontuacaoFornecedor.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******

function loadRelatorioPontuacaoFornecedor() {
    _pesquisaPontuacaoFornecedor = new PesquisaPontuacaoFornecedor();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridPontuacaoFornecedor = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/PontuacaoFornecedor/Pesquisa", _pesquisaPontuacaoFornecedor);

    _gridPontuacaoFornecedor.SetPermitirEdicaoColunas(true);
    _gridPontuacaoFornecedor.SetQuantidadeLinhasPorPagina(10);

    _relatorioPontuacaoFornecedor = new RelatorioGlobal("Relatorios/PontuacaoFornecedor/BuscarDadosRelatorio", _gridPontuacaoFornecedor, function () {
        _relatorioPontuacaoFornecedor.loadRelatorio(function () {
            KoBindings(_pesquisaPontuacaoFornecedor, "knockoutPesquisaPontuacaoFornecedor", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaPontuacaoFornecedor", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaPontuacaoFornecedor", false);

            // Buscas
            new BuscarFuncionario(_pesquisaPontuacaoFornecedor.Comprador);
            new BuscarClientes(_pesquisaPontuacaoFornecedor.Fornecedor);
            new BuscarOrdemCompra(_pesquisaPontuacaoFornecedor.OrdemCompra, RetornoOrdemCompra);
            new BuscarProdutoTMS(_pesquisaPontuacaoFornecedor.Produto);
            new BuscarNotaEntrada(_pesquisaPontuacaoFornecedor.NotaEntrada, RetornoNotaEntrada);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaPontuacaoFornecedor);
}

function RetornoOrdemCompra(data) {
    _pesquisaPontuacaoFornecedor.OrdemCompra.codEntity(data.Codigo);
    _pesquisaPontuacaoFornecedor.OrdemCompra.val(data.Numero);
}

function RetornoNotaEntrada(data) {
    _pesquisaPontuacaoFornecedor.NotaEntrada.codEntity(data.Codigo);
    _pesquisaPontuacaoFornecedor.NotaEntrada.val(data.Numero);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioPontuacaoFornecedor.gerarRelatorio("Relatorios/PontuacaoFornecedor/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioPontuacaoFornecedor.gerarRelatorio("Relatorios/PontuacaoFornecedor/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
