/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDRelatorio;
var _CRUDFiltrosRelatorio;
var _gridNivelServico;
var _pesquisaNivelServico;
var _relatorioNivelServico;

/*
 * Declaração das Classes
 */

var PesquisaNivelServico = function () {
    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Número da Carga: ", col: 12 });
    this.PlacaVeiculo = PropertyEntity({ text: "Placa: ", col: 12 });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", col: 12, dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", col: 12, getType: typesKnockout.date });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridNivelServico.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

/*
 * Declaração das Funções de Inicialização
 */

function LoadNivelServico() {
    _pesquisaNivelServico = new PesquisaNivelServico();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();
    _gridNivelServico = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/MonitoramentoNivelServico/Pesquisa", _pesquisaNivelServico);

    _gridNivelServico.SetPermitirEdicaoColunas(true);
    _gridNivelServico.SetCallbackEdicaoColunas(function () { _gridNivelServico.CarregarGrid(); });
    _gridNivelServico.SetQuantidadeLinhasPorPagina(20);

    _relatorioNivelServico = new RelatorioGlobal("Relatorios/MonitoramentoNivelServico/BuscarDadosRelatorio", _gridNivelServico, function () {
        _relatorioNivelServico.LoadRelatorio(function () {
            KoBindings(_pesquisaNivelServico, "knockoutPesquisaNivelServico", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaNivelServico", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaNivelServico", false);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaNivelServico.TipoRelatorio);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function GerarRelatorioPDFClick() {
    _relatorioNivelServico.GerarRelatorio("Relatorios/MonitoramentoNivelServico/GerarRelatorio", _pesquisaNivelServico, EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick() {
    _relatorioNivelServico.GerarRelatorio("Relatorios/MonitoramentoNivelServico/GerarRelatorio", _pesquisaNivelServico, EnumTipoArquivoRelatorio.XLS);
}