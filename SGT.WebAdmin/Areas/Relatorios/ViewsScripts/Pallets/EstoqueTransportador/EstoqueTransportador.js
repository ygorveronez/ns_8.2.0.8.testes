//*******MAPEAMENTO KNOUCKOUT*******

var _gridEstoqueTransportador, _pesquisaEstoqueTransportador, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioEstoqueTransportador;


var PesquisaEstoqueTransportador = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.Empresa = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFim = PropertyEntity({ text: "Data Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridEstoqueTransportador.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaEstoqueTransportador.Visible.visibleFade() == true) {
                _pesquisaEstoqueTransportador.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaEstoqueTransportador.Visible.visibleFade(true);
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

function LoadEstoqueTransportador() {
    _pesquisaEstoqueTransportador = new PesquisaEstoqueTransportador();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridEstoqueTransportador = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/EstoqueTransportador/Pesquisa", _pesquisaEstoqueTransportador);

    _gridEstoqueTransportador.SetPermitirEdicaoColunas(true);
    _gridEstoqueTransportador.SetQuantidadeLinhasPorPagina(20);

    _relatorioEstoqueTransportador = new RelatorioGlobal("Relatorios/EstoqueTransportador/BuscarDadosRelatorio", _gridEstoqueTransportador, function () {
        _relatorioEstoqueTransportador.loadRelatorio(function () {
            KoBindings(_pesquisaEstoqueTransportador, "knockoutPesquisaEstoqueTransportador", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaEstoqueTransportador", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaEstoqueTransportador", false);

            new BuscarTransportadores(_pesquisaEstoqueTransportador.Empresa);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaEstoqueTransportador);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe)
        _pesquisaEstoqueTransportador.Transportador.visible(false);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioEstoqueTransportador.gerarRelatorio("Relatorios/EstoqueTransportador/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioEstoqueTransportador.gerarRelatorio("Relatorios/EstoqueTransportador/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
