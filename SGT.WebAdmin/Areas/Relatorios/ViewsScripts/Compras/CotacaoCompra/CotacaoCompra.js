/// <reference path="../../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../../js/Global/CRUD.js" />
/// <reference path="../../../../../js/Global/Rest.js" />
/// <reference path="../../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../../js/Global/Grid.js" />
/// <reference path="../../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Produto.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioCotacaoCompra, _gridCotacaoCompra, _pesquisaCotacaoCompra, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaCotacaoCompra = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", issue: 2, getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", issue: 2, getType: typesKnockout.date });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Produto = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Produto:", idBtnSearch: guid() });
    this.Fornecedor = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Fornecedor:", idBtnSearch: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridCotacaoCompra.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******


function loadRelatorioCotacaoCompra() {

    _pesquisaCotacaoCompra = new PesquisaCotacaoCompra();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridCotacaoCompra = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/CotacaoCompra/Pesquisa", _pesquisaCotacaoCompra, null, null, 10);
    _gridCotacaoCompra.SetPermitirEdicaoColunas(true);

    _relatorioCotacaoCompra = new RelatorioGlobal("Relatorios/CotacaoCompra/BuscarDadosRelatorio", _gridCotacaoCompra, function () {
        _relatorioCotacaoCompra.loadRelatorio(function () {
            KoBindings(_pesquisaCotacaoCompra, "knockoutPesquisaCotacaoCompra");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaCotacaoCompra");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaCotacaoCompra");

            new BuscarClientes(_pesquisaCotacaoCompra.Fornecedor);
            new BuscarProdutoTMS(_pesquisaCotacaoCompra.Produto);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaCotacaoCompra);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioCotacaoCompra.gerarRelatorio("Relatorios/CotacaoCompra/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioCotacaoCompra.gerarRelatorio("Relatorios/CotacaoCompra/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}