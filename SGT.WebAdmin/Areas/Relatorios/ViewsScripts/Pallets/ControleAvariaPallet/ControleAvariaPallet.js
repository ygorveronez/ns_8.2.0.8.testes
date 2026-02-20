/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/SetorFuncionario.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/MotivoAvariaPallet.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDRelatorio;
var _CRUDFiltrosRelatorio;
var _gridControleAvariaPallet;
var _pesquisaControleAvariaPallet;
var _relatorioControleAvariaPallet;

/*
 * Declaração das Classes
 */

var PesquisaControleAvariaPallet = function () {
    var isTMS = (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS);

    this.DataInicio = PropertyEntity({ text: "Data Início: ", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ text: "Data Limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.ExibirQuantidadesAvariadas = PropertyEntity({ text: "Exibir Quantidades Avariadas?", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), issue: 0, visible: !isTMS });
    this.MotivoAvaria = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Motivo Avaria:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Setor = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Setor:", idBtnSearch: guid(), issue: 0, visible: !isTMS });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoAvariaPallet.Todas), options: EnumSituacaoAvariaPallet.obterOpcoes(), def: EnumSituacaoAvariaPallet.Todas, text: "Situação: " });
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Empresa/Filial:", idBtnSearch: guid(), visible: isTMS });

    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridControleAvariaPallet.CarregarGrid();
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

function LoadControleAvariaPallet() {
    _pesquisaControleAvariaPallet = new PesquisaControleAvariaPallet();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();
    _gridControleAvariaPallet = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ControleAvariaPallet/Pesquisa", _pesquisaControleAvariaPallet);

    _gridControleAvariaPallet.SetPermitirEdicaoColunas(true);
    _gridControleAvariaPallet.SetQuantidadeLinhasPorPagina(20);

    _relatorioControleAvariaPallet = new RelatorioGlobal("Relatorios/ControleAvariaPallet/BuscarDadosRelatorio", _gridControleAvariaPallet, function () {
        _relatorioControleAvariaPallet.loadRelatorio(function () {
            KoBindings(_pesquisaControleAvariaPallet, "knockoutPesquisaControleAvariaPallet", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaControleAvariaPallet", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaControleAvariaPallet", false);

            new BuscarFilial(_pesquisaControleAvariaPallet.Filial);
            new BuscarMotivoAvariaPallet(_pesquisaControleAvariaPallet.MotivoAvaria);
            new BuscarSetorFuncionario(_pesquisaControleAvariaPallet.Setor);
            new BuscarTransportadores(_pesquisaControleAvariaPallet.Transportador);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaControleAvariaPallet);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function GerarRelatorioPDFClick() {
    _relatorioControleAvariaPallet.gerarRelatorio("Relatorios/ControleAvariaPallet/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick() {
    _relatorioControleAvariaPallet.gerarRelatorio("Relatorios/ControleAvariaPallet/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
