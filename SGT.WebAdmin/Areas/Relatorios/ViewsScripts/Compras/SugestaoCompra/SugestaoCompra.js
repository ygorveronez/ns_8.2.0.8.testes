//*******MAPEAMENTO KNOUCKOUT*******

var _gridSugestaoCompra, _pesquisaSugestaoCompra, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioSugestaoCompra;


var PesquisaSugestaoCompra = function () {
    this.Produto = PropertyEntity({ text: "Produto:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid() });
    this.Empresa = PropertyEntity({ text: "Empresa:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoProduto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Produto:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });

    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridSugestaoCompra.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaSugestaoCompra.Visible.visibleFade() == true) {
                _pesquisaSugestaoCompra.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaSugestaoCompra.Visible.visibleFade(true);
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

function loadRelatorioSugestaoCompra() {
    _pesquisaSugestaoCompra = new PesquisaSugestaoCompra();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridSugestaoCompra = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/SugestaoCompra/Pesquisa", _pesquisaSugestaoCompra);

    _gridSugestaoCompra.SetPermitirEdicaoColunas(true);
    _gridSugestaoCompra.SetQuantidadeLinhasPorPagina(10);

    _relatorioSugestaoCompra = new RelatorioGlobal("Relatorios/SugestaoCompra/BuscarDadosRelatorio", _gridSugestaoCompra, function () {
        _relatorioSugestaoCompra.loadRelatorio(function () {
            KoBindings(_pesquisaSugestaoCompra, "knockoutPesquisaSugestaoCompra", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaSugestaoCompra", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaSugestaoCompra", false);

            // Buscas            
            new BuscarProdutoTMS(_pesquisaSugestaoCompra.Produto);
            new BuscarEmpresa(_pesquisaSugestaoCompra.Empresa);
            new BuscarGruposProdutosTMS(_pesquisaSugestaoCompra.GrupoProduto, null);
            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiNFe)
                _pesquisaSugestaoCompra.Empresa.visible(false);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaSugestaoCompra);
}

function RetornoOrdemCompra(data) {
    _pesquisaSugestaoCompra.OrdemCompra.codEntity(data.Codigo);
    _pesquisaSugestaoCompra.OrdemCompra.val(data.Numero);
}

function RetornoNotaEntrada(data) {
    _pesquisaSugestaoCompra.NotaEntrada.codEntity(data.Codigo);
    _pesquisaSugestaoCompra.NotaEntrada.val(data.Numero);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioSugestaoCompra.gerarRelatorio("Relatorios/SugestaoCompra/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioSugestaoCompra.gerarRelatorio("Relatorios/SugestaoCompra/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
