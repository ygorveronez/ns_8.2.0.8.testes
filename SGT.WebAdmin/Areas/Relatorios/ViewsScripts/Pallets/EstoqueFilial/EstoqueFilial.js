//*******MAPEAMENTO KNOUCKOUT*******

var _gridEstoqueFilial, _pesquisaEstoqueFilial, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioEstoqueFilial;


var PesquisaEstoqueFilial = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFim = PropertyEntity({ text: "Data Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridEstoqueFilial.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaEstoqueFilial.Visible.visibleFade() == true) {
                _pesquisaEstoqueFilial.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaEstoqueFilial.Visible.visibleFade(true);
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

function LoadEstoqueFilial() {
    _pesquisaEstoqueFilial = new PesquisaEstoqueFilial();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridEstoqueFilial = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/EstoqueFilial/Pesquisa", _pesquisaEstoqueFilial);

    _gridEstoqueFilial.SetPermitirEdicaoColunas(true);
    _gridEstoqueFilial.SetQuantidadeLinhasPorPagina(20);

    _relatorioEstoqueFilial = new RelatorioGlobal("Relatorios/EstoqueFilial/BuscarDadosRelatorio", _gridEstoqueFilial, function () {
        _relatorioEstoqueFilial.loadRelatorio(function () {
            KoBindings(_pesquisaEstoqueFilial, "knockoutPesquisaEstoqueFilial", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaEstoqueFilial", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaEstoqueFilial", false);

            new BuscarFilial(_pesquisaEstoqueFilial.Filial);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaEstoqueFilial);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe)
        _pesquisaEstoqueFilial.Transportador.visible(false);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioEstoqueFilial.gerarRelatorio("Relatorios/EstoqueFilial/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioEstoqueFilial.gerarRelatorio("Relatorios/EstoqueFilial/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
