/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDRelatorio;
var _CRUDFiltrosRelatorio;
var _gridPosicaoDaFrota;
var _pesquisaPosicaoDaFrota;
var _relatorioPosicaoDaFrota;

/*
 * Declaração das Classes
 */

var PesquisaPosicaoDaFrota = function () {
    this.PlacaVeiculo = PropertyEntity({ text: "Placa: ", col: 12 });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridPosicaoDaFrota.CarregarGrid();
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

function LoadPosicaoDaFrota() {
    _pesquisaPosicaoDaFrota = new PesquisaPosicaoDaFrota();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();
    _gridPosicaoDaFrota = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/MonitoramentoPosicaoDaFrota/Pesquisa", _pesquisaPosicaoDaFrota);

    _gridPosicaoDaFrota.SetPermitirEdicaoColunas(true);
    _gridPosicaoDaFrota.SetQuantidadeLinhasPorPagina(20);

    _relatorioPosicaoDaFrota = new RelatorioGlobal("Relatorios/MonitoramentoPosicaoDaFrota/BuscarDadosRelatorio", _gridPosicaoDaFrota, function () {
        _relatorioPosicaoDaFrota.loadRelatorio(function () {
            KoBindings(_pesquisaPosicaoDaFrota, "knockoutPesquisaPosicaoDaFrota", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaPosicaoDaFrota", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaPosicaoDaFrota", false);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaPosicaoDaFrota);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function GerarRelatorioPDFClick() {
    _relatorioPosicaoDaFrota.gerarRelatorio("Relatorios/MonitoramentoPosicaoDaFrota/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick() {
    _relatorioPosicaoDaFrota.gerarRelatorio("Relatorios/MonitoramentoPosicaoDaFrota/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}