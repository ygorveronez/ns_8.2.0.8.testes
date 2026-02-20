/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDRelatorio;
var _CRUDFiltrosRelatorio;
var _gridEstoqueCompraPallet;
var _pesquisaEstoqueCompraPallet;
var _relatorioEstoqueCompraPallet;

/*
 * Declaração das Classes
 */

var PesquisaEstoqueCompraPallet = function () {
    var isTMS = _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS;

    this.DataInicio = PropertyEntity({ text: "Data Início: ", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ text: "Data Limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), issue: 0, visible: !isTMS });
    this.Fornecedor = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Fornecedor:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Empresa/Filial:", idBtnSearch: guid(), issue: 0, visible: isTMS });
    this.Nfe = PropertyEntity({ text: "NF-e: ", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" } });

    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridEstoqueCompraPallet.CarregarGrid();
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


function LoadEstoqueCompraPallet() {
    _pesquisaEstoqueCompraPallet = new PesquisaEstoqueCompraPallet();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();
    _gridEstoqueCompraPallet = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/EstoqueCompraPallet/Pesquisa", _pesquisaEstoqueCompraPallet);

    _gridEstoqueCompraPallet.SetPermitirEdicaoColunas(true);
    _gridEstoqueCompraPallet.SetQuantidadeLinhasPorPagina(20);

    _relatorioEstoqueCompraPallet = new RelatorioGlobal("Relatorios/EstoqueCompraPallet/BuscarDadosRelatorio", _gridEstoqueCompraPallet, function () {
        _relatorioEstoqueCompraPallet.loadRelatorio(function () {
            KoBindings(_pesquisaEstoqueCompraPallet, "knockoutPesquisaEstoqueCompraPallet", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaEstoqueCompraPallet", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaEstoqueCompraPallet", false);

            new BuscarClientes(_pesquisaEstoqueCompraPallet.Fornecedor);
            new BuscarFilial(_pesquisaEstoqueCompraPallet.Filial);
            new BuscarTransportadores(_pesquisaEstoqueCompraPallet.Transportador);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaEstoqueCompraPallet);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function GerarRelatorioPDFClick() {
    _relatorioEstoqueCompraPallet.gerarRelatorio("Relatorios/EstoqueCompraPallet/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick() {
    _relatorioEstoqueCompraPallet.gerarRelatorio("Relatorios/EstoqueCompraPallet/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
