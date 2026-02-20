/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumAlertaMonitorStatus.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoAlerta.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDRelatorioAlerta;
var _CRUDFiltrosRelatorioAlerta;
var _gridAlerta;
var _pesquisaAlerta;
var _relatorioAlerta;

/*
 * Declaração das Classes
 */

var PesquisaAlerta = function () {
    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Número da Carga:" });
    this.Placa = PropertyEntity({ text: "Placa:" });
    this.Motorista = PropertyEntity({ text: "Motorista:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ text: "Transportador:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.AlertaMonitorStatus = PropertyEntity({ text: "Status:", val: ko.observable(null), options: EnumAlertaMonitorStatus.obterOpcoesPesquisa(), def: null });
    this.TipoAlerta = PropertyEntity({ text: "Tipo:", val: ko.observable(null), options: EnumTipoAlerta.obterOpcoesPesquisa(), def: null });
    this.ExibirApenasComPosicaoTardia = PropertyEntity({ text: "Com Posição Retroativa", getType: typesKnockout.bool, visible: ko.observable(false), val: ko.observable(false) });
    this.DataInicial = PropertyEntity({ text: "Data do evento do alerta inicial:", getType: typesKnockout.dateTime });
    this.DataFinal = PropertyEntity({ text: "Data do evento do alerta final:", getType: typesKnockout.dateTime });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

}

var CRUDFiltrosRelatorioAlerta = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridAlerta.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorioAlerta = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

/*
 * Declaração das Funções de Inicialização
 */

function LoadMonitoramentoAlerta() {
    _pesquisaAlerta = new PesquisaAlerta();

    _pesquisaAlerta.TipoAlerta.val.subscribe(exibeCamposFiltro);

    _CRUDRelatorioAlerta = new CRUDRelatorioAlerta();
    _CRUDFiltrosRelatorioAlerta = new CRUDFiltrosRelatorioAlerta();
    _gridAlerta = new GridView(_CRUDFiltrosRelatorioAlerta.Preview.idGrid, "Relatorios/MonitoramentoAlerta/Pesquisa", _pesquisaAlerta);

    _gridAlerta.SetPermitirEdicaoColunas(true);
    _gridAlerta.SetQuantidadeLinhasPorPagina(20);

    _relatorioAlerta = new RelatorioGlobal("Relatorios/MonitoramentoAlerta/BuscarDadosRelatorio", _gridAlerta, function () {
        _relatorioAlerta.loadRelatorio(function () {
            KoBindings(_pesquisaAlerta, "knockoutPesquisaAlerta", false);
            new BuscarTransportadores(_pesquisaAlerta.Transportador, null, null, true);
            new BuscarFuncionario(_pesquisaAlerta.Motorista);

            KoBindings(_CRUDRelatorioAlerta, "knockoutCRUDPesquisaAlerta", false);
            KoBindings(_CRUDFiltrosRelatorioAlerta, "knockoutCRUDFiltrosPesquisaAlerta", false);
            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaAlerta);
}



function exibeCamposFiltro(val) {
    if (val == EnumTipoAlerta.SemSinal || val == EnumTipoAlerta.PerdaDeSinal)
        _pesquisaAlerta.ExibirApenasComPosicaoTardia.visible(true);
    else
        _pesquisaAlerta.ExibirApenasComPosicaoTardia.visible(false);
}


/*
 * Declaração das Funções Associadas a Eventos
 */

function GerarRelatorioPDFClick() {
    _relatorioAlerta.gerarRelatorio("Relatorios/MonitoramentoAlerta/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick() {
    _relatorioAlerta.gerarRelatorio("Relatorios/MonitoramentoAlerta/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}