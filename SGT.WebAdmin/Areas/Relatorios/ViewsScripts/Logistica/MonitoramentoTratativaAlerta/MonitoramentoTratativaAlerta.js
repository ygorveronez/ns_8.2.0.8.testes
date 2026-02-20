/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDRelatorio;
var _CRUDFiltrosRelatorio;
var _gridTratativaAlerta;
var _pesquisaTratativaAlerta;
var _relatorioTratativaAlerta;

/*
 * Declaração das Classes
 */

var PesquisaTratativaAlerta = function () {
    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Número da Carga: ", col: 12 });
    this.PlacaVeiculo = PropertyEntity({ text: "Placa: ", col: 12 });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", col: 12, dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", col: 12, getType: typesKnockout.date });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridTratativaAlerta.CarregarGrid();
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

function LoadTratativaAlerta() {
    _pesquisaTratativaAlerta = new PesquisaTratativaAlerta();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();
    _gridTratativaAlerta = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/MonitoramentoTratativaAlerta/Pesquisa", _pesquisaTratativaAlerta);

    _gridTratativaAlerta.SetPermitirEdicaoColunas(true);
    _gridTratativaAlerta.SetQuantidadeLinhasPorPagina(20);

    _relatorioTratativaAlerta = new RelatorioGlobal("Relatorios/MonitoramentoTratativaAlerta/BuscarDadosRelatorio", _gridTratativaAlerta, function () {
        _relatorioTratativaAlerta.loadRelatorio(function () {
            KoBindings(_pesquisaTratativaAlerta, "knockoutPesquisaTratativaAlerta", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaTratativaAlerta", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaTratativaAlerta", false);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaTratativaAlerta);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function GerarRelatorioPDFClick() {
    _relatorioTratativaAlerta.gerarRelatorio("Relatorios/MonitoramentoTratativaAlerta/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick() {
    _relatorioTratativaAlerta.gerarRelatorio("Relatorios/MonitoramentoTratativaAlerta/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}