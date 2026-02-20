/// <reference path="../../../../../ViewsScripts/Consultas/SetorFuncionario.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Turno.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoTransferenciaPallet.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDRelatorio;
var _CRUDFiltrosRelatorio;
var _gridControleTransferenciaPallet;
var _pesquisaControleTransferenciaPallet;
var _relatorioControleTransferenciaPallet;

/*
 * Declaração das Classes
 */

var PesquisaControleTransferenciaPallet = function () {
    this.DataInicio = PropertyEntity({ text: "Data Solicitação Início: ", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ text: "Data Solicitação Limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Setor = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Setor:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Turno = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Turno:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoTransferenciaPallet.Todas), options: EnumSituacaoTransferenciaPallet.obterOpcoes(), def: EnumSituacaoTransferenciaPallet.Todas, text: "Situação: " });

    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridControleTransferenciaPallet.CarregarGrid();
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

function LoadControleTransferenciaPallet() {
    _pesquisaControleTransferenciaPallet = new PesquisaControleTransferenciaPallet();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();
    _gridControleTransferenciaPallet = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ControleTransferenciaPallet/Pesquisa", _pesquisaControleTransferenciaPallet);

    _gridControleTransferenciaPallet.SetPermitirEdicaoColunas(true);
    _gridControleTransferenciaPallet.SetQuantidadeLinhasPorPagina(20);

    _relatorioControleTransferenciaPallet = new RelatorioGlobal("Relatorios/ControleTransferenciaPallet/BuscarDadosRelatorio", _gridControleTransferenciaPallet, function () {
        _relatorioControleTransferenciaPallet.loadRelatorio(function () {
            KoBindings(_pesquisaControleTransferenciaPallet, "knockoutPesquisaControleTransferenciaPallet", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaControleTransferenciaPallet", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaControleTransferenciaPallet", false);

            new BuscarFilial(_pesquisaControleTransferenciaPallet.Filial);
            new BuscarSetorFuncionario(_pesquisaControleTransferenciaPallet.Setor);
            new BuscarTurno(_pesquisaControleTransferenciaPallet.Turno);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaControleTransferenciaPallet);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function GerarRelatorioPDFClick() {
    _relatorioControleTransferenciaPallet.gerarRelatorio("Relatorios/ControleTransferenciaPallet/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick() {
    _relatorioControleTransferenciaPallet.gerarRelatorio("Relatorios/ControleTransferenciaPallet/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
