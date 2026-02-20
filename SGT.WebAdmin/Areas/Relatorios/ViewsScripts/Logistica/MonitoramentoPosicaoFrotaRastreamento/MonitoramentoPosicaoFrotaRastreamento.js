/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDRelatorio;
var _CRUDFiltrosRelatorio;
var _gridPosicaoFrotaRastreamento;
var _pesquisaPosicaoFrotaRastreamento;
var _relatorioPosicaoFrotaRastreamento;

/*
 * Declaração das Classes
 */

var PesquisaPosicaoFrotaRastreamento = function () {
    this.Veiculo = PropertyEntity({ text: "Veículo:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ text: "Transportador:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.DataInicial = PropertyEntity({ text: "Data inicial:", getType: typesKnockout.dateTime, val: ko.observable(Global.PrimeiraDataDoMesAtual()), required: true });
    this.DataFinal = PropertyEntity({ text: "Data final:", getType: typesKnockout.dateTime, val: ko.observable(Global.UltimaDataDoMesAtual()), required: true });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            if (ValidarCamposObrigatorios(_pesquisaPosicaoFrotaRastreamento)) {
                _gridPosicaoFrotaRastreamento.CarregarGrid();
            }
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

function LoadPosicaoFrotaRastreamento() {
    _pesquisaPosicaoFrotaRastreamento = new PesquisaPosicaoFrotaRastreamento();

    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridPosicaoFrotaRastreamento = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/MonitoramentoPosicaoFrotaRastreamento/Pesquisa", _pesquisaPosicaoFrotaRastreamento, null, null, 50, null, true, null, null, 1000000);
    _gridPosicaoFrotaRastreamento.SetPermitirEdicaoColunas(true);
    _gridPosicaoFrotaRastreamento.SetQuantidadeLinhasPorPagina(20);

    _relatorioPosicaoFrotaRastreamento = new RelatorioGlobal(
        "Relatorios/MonitoramentoPosicaoFrotaRastreamento/BuscarDadosRelatorio",
        _gridPosicaoFrotaRastreamento,
        function () {
            _relatorioPosicaoFrotaRastreamento.loadRelatorio(function () {
                KoBindings(_pesquisaPosicaoFrotaRastreamento, "knockoutPesquisaPosicaoFrotaRastreamento", false);
                KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaPosicaoFrotaRastreamento", false);
                KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaPosicaoFrotaRastreamento", false);
                new BuscarTransportadores(_pesquisaPosicaoFrotaRastreamento.Transportador, null, null, true);
                new BuscarVeiculos(_pesquisaPosicaoFrotaRastreamento.Veiculo);
                $("#divConteudoRelatorio").show();
            });
        },
        null,
        "knockoutCRUDConfiguracaoRelatorio",
        _pesquisaPosicaoFrotaRastreamento
    );
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function GerarRelatorioPDFClick() {
    _relatorioPosicaoFrotaRastreamento.gerarRelatorio("Relatorios/MonitoramentoPosicaoFrotaRastreamento/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick() {
    _relatorioPosicaoFrotaRastreamento.gerarRelatorio("Relatorios/MonitoramentoPosicaoFrotaRastreamento/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}