/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDRelatorio;
var _CRUDFiltrosRelatorio;
var _gridVeiculoAlvo;
var _pesquisaVeiculoAlvo;
var _relatorioVeiculoAlvo;

/*
 * Declaração das Classes
 */

var PesquisaVeiculoAlvo = function () {
    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Número da Carga: ", col: 12 });
    this.PlacaVeiculo = PropertyEntity({ text: "Placa: ", col: 12 });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", col: 12, dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", col: 12, getType: typesKnockout.date });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridVeiculoAlvo.CarregarGrid();
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

function LoadVeiculoAlvo() {
    _pesquisaVeiculoAlvo = new PesquisaVeiculoAlvo();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();
    _gridVeiculoAlvo = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/MonitoramentoVeiculoAlvo/Pesquisa", _pesquisaVeiculoAlvo);

    _gridVeiculoAlvo.SetPermitirEdicaoColunas(true);
    _gridVeiculoAlvo.SetQuantidadeLinhasPorPagina(20);

    _relatorioVeiculoAlvo = new RelatorioGlobal("Relatorios/MonitoramentoVeiculoAlvo/BuscarDadosRelatorio", _gridVeiculoAlvo, function () {
        _relatorioVeiculoAlvo.loadRelatorio(function () {
            KoBindings(_pesquisaVeiculoAlvo, "knockoutPesquisaVeiculoAlvo", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaVeiculoAlvo", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaVeiculoAlvo", false);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaVeiculoAlvo);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function GerarRelatorioPDFClick() {
    _relatorioVeiculoAlvo.gerarRelatorio("Relatorios/MonitoramentoVeiculoAlvo/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick() {
    _relatorioVeiculoAlvo.gerarRelatorio("Relatorios/MonitoramentoVeiculoAlvo/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}