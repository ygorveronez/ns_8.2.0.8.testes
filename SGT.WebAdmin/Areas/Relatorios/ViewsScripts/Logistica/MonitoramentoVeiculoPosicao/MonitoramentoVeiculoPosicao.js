/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDRelatorio;
var _CRUDFiltrosRelatorio;
var _gridVeiculoPosicao;
var _pesquisaVeiculoPosicao;
var _relatorioVeiculoPosicao;

/*
 * Declaração das Classes
 */

var PesquisaVeiculoPosicao = function () {
    this.PlacaVeiculo = PropertyEntity({ text: "Placa: ", col: 12 });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", col: 12, dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", col: 12, getType: typesKnockout.date });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridVeiculoPosicao.CarregarGrid();
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

function LoadVeiculoPosicao() {
    _pesquisaVeiculoPosicao = new PesquisaVeiculoPosicao();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();
    _gridVeiculoPosicao = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/MonitoramentoVeiculoPosicao/Pesquisa", _pesquisaVeiculoPosicao);

    _gridVeiculoPosicao.SetPermitirEdicaoColunas(true);
    _gridVeiculoPosicao.SetQuantidadeLinhasPorPagina(20);

    _relatorioVeiculoPosicao = new RelatorioGlobal("Relatorios/MonitoramentoVeiculoPosicao/BuscarDadosRelatorio", _gridVeiculoPosicao, function () {
        _relatorioVeiculoPosicao.loadRelatorio(function () {
            KoBindings(_pesquisaVeiculoPosicao, "knockoutPesquisaVeiculoPosicao", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaVeiculoPosicao", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaVeiculoPosicao", false);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaVeiculoPosicao);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function GerarRelatorioPDFClick() {
    _relatorioVeiculoPosicao.gerarRelatorio("Relatorios/MonitoramentoVeiculoPosicao/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick() {
    _relatorioVeiculoPosicao.gerarRelatorio("Relatorios/MonitoramentoVeiculoPosicao/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}