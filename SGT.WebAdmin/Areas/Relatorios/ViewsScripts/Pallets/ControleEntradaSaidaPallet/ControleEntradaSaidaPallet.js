/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumNaturezaMovimentacaoEstoquePallet.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDRelatorio;
var _CRUDFiltrosRelatorio;
var _gridControleEntradaSaidaPallet;
var _pesquisaControleEntradaSaidaPallet;
var _relatorioControleEntradaSaidaPallet;

/*
 * Declaração das Classes
 */

var PesquisaControleEntradaSaidaPallet = function () {
    this.Cliente = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid(), issue: 0, visible: true });
    this.DataInicio = PropertyEntity({ text: "Data Início: ", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ text: "Data Limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), issue: 0, visible: (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS) });
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS ? "Empresa/Filial" : "Transportador:"), idBtnSearch: guid(), issue: 0, visible: true });
    this.NaturezaMovimentacao = PropertyEntity({ val: ko.observable(EnumNaturezaMovimentacaoEstoquePallet.Todas), options: EnumNaturezaMovimentacaoEstoquePallet.obterOpcoes(), def: EnumNaturezaMovimentacaoEstoquePallet.Todas, text: "Natureza da Operação: " });

    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridControleEntradaSaidaPallet.CarregarGrid();
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

function LoadControleEntradaSaidaPallet() {
    _pesquisaControleEntradaSaidaPallet = new PesquisaControleEntradaSaidaPallet();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();
    _gridControleEntradaSaidaPallet = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ControleEntradaSaidaPallet/Pesquisa", _pesquisaControleEntradaSaidaPallet);

    _gridControleEntradaSaidaPallet.SetPermitirEdicaoColunas(true);
    _gridControleEntradaSaidaPallet.SetQuantidadeLinhasPorPagina(20);

    _relatorioControleEntradaSaidaPallet = new RelatorioGlobal("Relatorios/ControleEntradaSaidaPallet/BuscarDadosRelatorio", _gridControleEntradaSaidaPallet, function () {
        _relatorioControleEntradaSaidaPallet.loadRelatorio(function () {
            KoBindings(_pesquisaControleEntradaSaidaPallet, "knockoutPesquisaControleEntradaSaidaPallet", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaControleEntradaSaidaPallet", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaControleEntradaSaidaPallet", false);

            new BuscarClientes(_pesquisaControleEntradaSaidaPallet.Cliente);
            new BuscarFilial(_pesquisaControleEntradaSaidaPallet.Filial);
            new BuscarTransportadores(_pesquisaControleEntradaSaidaPallet.Transportador);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaControleEntradaSaidaPallet);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function GerarRelatorioPDFClick() {
    _relatorioControleEntradaSaidaPallet.gerarRelatorio("Relatorios/ControleEntradaSaidaPallet/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick() {
    _relatorioControleEntradaSaidaPallet.gerarRelatorio("Relatorios/ControleEntradaSaidaPallet/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
