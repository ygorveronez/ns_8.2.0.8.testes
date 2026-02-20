//*******MAPEAMENTO KNOUCKOUT*******

var _gridFechamentoTransportador, _pesquisaFechamentoTransportador, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioFechamentoTransportador;


var PesquisaFechamentoTransportador = function () {
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
            _gridFechamentoTransportador.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaFechamentoTransportador.Visible.visibleFade() == true) {
                _pesquisaFechamentoTransportador.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaFechamentoTransportador.Visible.visibleFade(true);
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

function LoadFechamentoTransportador() {
    _pesquisaFechamentoTransportador = new PesquisaFechamentoTransportador();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridFechamentoTransportador = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/FechamentoTransportador/Pesquisa", _pesquisaFechamentoTransportador);

    _gridFechamentoTransportador.SetPermitirEdicaoColunas(true);
    _gridFechamentoTransportador.SetQuantidadeLinhasPorPagina(20);

    _relatorioFechamentoTransportador = new RelatorioGlobal("Relatorios/FechamentoTransportador/BuscarDadosRelatorio", _gridFechamentoTransportador, function () {
        _relatorioFechamentoTransportador.loadRelatorio(function () {
            KoBindings(_pesquisaFechamentoTransportador, "knockoutPesquisaFechamentoTransportador", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaFechamentoTransportador", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaFechamentoTransportador", false);

            new BuscarTransportadores(_pesquisaFechamentoTransportador.Empresa);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaFechamentoTransportador);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe)
        _pesquisaFechamentoTransportador.Transportador.visible(false);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioFechamentoTransportador.gerarRelatorio("Relatorios/FechamentoTransportador/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioFechamentoTransportador.gerarRelatorio("Relatorios/FechamentoTransportador/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
