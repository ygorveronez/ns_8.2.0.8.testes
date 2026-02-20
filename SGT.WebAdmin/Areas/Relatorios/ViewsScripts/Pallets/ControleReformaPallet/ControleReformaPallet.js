/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDRelatorio;
var _CRUDFiltrosRelatorio;
var _gridControleReformaPallet;
var _pesquisaControleReformaPallet;
var _relatorioControleReformaPallet;

/*
 * Declaração das Classes
 */

var PesquisaControleReformaPallet = function () {
    var isTMS = _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS;

    this.DataInicio = PropertyEntity({ text: "Data Início: ", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ text: "Data Limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.DataRetornoInicio = PropertyEntity({ text: "Data Retorno Início: ", getType: typesKnockout.date });
    this.DataRetornoLimite = PropertyEntity({ text: "Data Retorno Limite: ", dateRangeInit: this.DataRetornoInicio, getType: typesKnockout.date });
    this.ExibirNfs = PropertyEntity({ text: "Exibir NFS de Retorno?", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), issue: 0, visible: !isTMS });
    this.Fornecedor = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Fornecedor:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Empresa/Filial:", idBtnSearch: guid(), issue: 0, visible: isTMS });
    this.Nfe = PropertyEntity({ text: "NF-e: ", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.NfeRetorno = PropertyEntity({ text: "NF-e Retorno: ", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" } });

    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;
    this.DataRetornoInicio.dateRangeLimit = this.DataRetornoLimite;
    this.DataRetornoLimite.dateRangeInit = this.DataRetornoInicio;

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridControleReformaPallet.CarregarGrid();
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

function LoadControleReformaPallet() {
    _pesquisaControleReformaPallet = new PesquisaControleReformaPallet();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();
    _gridControleReformaPallet = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ControleReformaPallet/Pesquisa", _pesquisaControleReformaPallet);

    _gridControleReformaPallet.SetPermitirEdicaoColunas(true);
    _gridControleReformaPallet.SetQuantidadeLinhasPorPagina(20);

    _relatorioControleReformaPallet = new RelatorioGlobal("Relatorios/ControleReformaPallet/BuscarDadosRelatorio", _gridControleReformaPallet, function () {
        _relatorioControleReformaPallet.loadRelatorio(function () {
            KoBindings(_pesquisaControleReformaPallet, "knockoutPesquisaControleReformaPallet", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaControleReformaPallet", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaControleReformaPallet", false);

            new BuscarFilial(_pesquisaControleReformaPallet.Filial);
            new BuscarClientes(_pesquisaControleReformaPallet.Fornecedor);
            new BuscarTransportadores(_pesquisaControleReformaPallet.Transportador);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaControleReformaPallet);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function GerarRelatorioPDFClick() {
    _relatorioControleReformaPallet.gerarRelatorio("Relatorios/ControleReformaPallet/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick() {
    _relatorioControleReformaPallet.gerarRelatorio("Relatorios/ControleReformaPallet/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
