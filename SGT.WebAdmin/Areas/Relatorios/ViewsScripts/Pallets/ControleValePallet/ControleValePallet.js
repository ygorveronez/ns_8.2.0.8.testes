/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoValePallet.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDRelatorio;
var _CRUDFiltrosRelatorio;
var _gridControleValePallet;
var _pesquisaControleValePallet;
var _relatorioControleValePallet;

/*
 * Declaração das Classes
 */

var PesquisaControleValePallet = function () {
    this.Cliente = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.DataInicio = PropertyEntity({ text: "Data Solicitação Início: ", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ text: "Data Solicitação Limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Nfe = PropertyEntity({ text: "NF-e: ", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoValePallet.Todas), options: EnumSituacaoValePallet.obterOpcoes(), def: EnumSituacaoValePallet.Todas, text: "Situação: " });

    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridControleValePallet.CarregarGrid();
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

function LoadControleValePallet() {
    _pesquisaControleValePallet = new PesquisaControleValePallet();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();
    _gridControleValePallet = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ControleValePallet/Pesquisa", _pesquisaControleValePallet);

    _gridControleValePallet.SetPermitirEdicaoColunas(true);
    _gridControleValePallet.SetQuantidadeLinhasPorPagina(20);

    _relatorioControleValePallet = new RelatorioGlobal("Relatorios/ControleValePallet/BuscarDadosRelatorio", _gridControleValePallet, function () {
        _relatorioControleValePallet.loadRelatorio(function () {
            KoBindings(_pesquisaControleValePallet, "knockoutPesquisaControleValePallet", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaControleValePallet", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaControleValePallet", false);

            new BuscarClientes(_pesquisaControleValePallet.Cliente);
            new BuscarFilial(_pesquisaControleValePallet.Filial);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaControleValePallet);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function GerarRelatorioPDFClick() {
    _relatorioControleValePallet.gerarRelatorio("Relatorios/ControleValePallet/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick() {
    _relatorioControleValePallet.gerarRelatorio("Relatorios/ControleValePallet/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
