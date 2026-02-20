/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDRelatorioMonitoramentoEntrega;
var _CRUDFiltrosRelatorioMonitoramentoEntrega;
var _gridMonitoramentoEntrega;
var _pesquisaMonitoramentoEntrega;
var _relatorioMonitoramentoEntrega;

/*
 * Declaração das Classes
 */

var PesquisaMonitoramentoEntrega = function () {
    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Número da Carga:" });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo operação:", idBtnSearch: guid() });
    this.NumeroNotaFiscal = PropertyEntity({ text: "Nota Fiscal:", val: ko.observable(0) });
    this.NumeroPedido = PropertyEntity({ text: "Número Pedido:", val: ko.observable("") });
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorioMonitoramentoEntrega = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridMonitoramentoEntrega.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorioMonitoramentoEntrega = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

/*
 * Declaração das Funções de Inicialização
 */

function LoadMonitoramentoEntrega() {
    _pesquisaMonitoramentoEntrega = new PesquisaMonitoramentoEntrega();

    _CRUDRelatorioMonitoramentoEntrega = new CRUDRelatorioMonitoramentoEntrega();
    _CRUDFiltrosRelatorioMonitoramentoEntrega = new CRUDFiltrosRelatorioMonitoramentoEntrega();
    _gridMonitoramentoEntrega = new GridView(_CRUDFiltrosRelatorioMonitoramentoEntrega.Preview.idGrid, "Relatorios/MonitoramentoControleEntrega/Pesquisa", _pesquisaMonitoramentoEntrega);

    _gridMonitoramentoEntrega.SetPermitirEdicaoColunas(true);
    _gridMonitoramentoEntrega.SetQuantidadeLinhasPorPagina(20);

    _relatorioMonitoramentoEntrega = new RelatorioGlobal("Relatorios/MonitoramentoControleEntrega/BuscarDadosRelatorio", _gridMonitoramentoEntrega, function () {
        _relatorioMonitoramentoEntrega.loadRelatorio(function () {
            KoBindings(_pesquisaMonitoramentoEntrega, "knockoutPesquisaMonitoramentoEntrega", false);

            new BuscarVeiculos(_pesquisaMonitoramentoEntrega.Veiculo);
            new BuscarFilial(_pesquisaMonitoramentoEntrega.Filial);
            new BuscarTiposOperacao(_pesquisaMonitoramentoEntrega.TipoOperacao);

            KoBindings(_CRUDRelatorioMonitoramentoEntrega, "knockoutCRUDPesquisaMonitoramentoEntrega", false);
            KoBindings(_CRUDFiltrosRelatorioMonitoramentoEntrega, "knockoutCRUDFiltrosPesquisaMonitoramentoEntrega", false);
            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaMonitoramentoEntrega);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function GerarRelatorioPDFClick() {
    _relatorioMonitoramentoEntrega.gerarRelatorio("Relatorios/MonitoramentoControleEntrega/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick() {
    _relatorioMonitoramentoEntrega.gerarRelatorio("Relatorios/MonitoramentoControleEntrega/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}