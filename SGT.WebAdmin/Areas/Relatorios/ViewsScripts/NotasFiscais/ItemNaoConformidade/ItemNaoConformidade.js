/// <reference path="../../../../../js/app.config.js" />
/// <reference path="../../../../../js/Global/CRUD.js" />
/// <reference path="../../../../../js/Global/Grid.js" />
/// <reference path="../../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../../js/Global/Rest.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Carga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoOcorrencia.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridItemNaoConformidade;
var _pesquisaItemNaoConformidade;
var _CRUDRelatorio;
var _CRUDFiltrosRelatorio;
var _relatorioItemNaoConformidade;

var _configuracaoNumeroInteiro = {
    precision: 0,
    allowZero: false,
    allowNegative: false,
    thousands: ""
};

/*
 * Declaração das Classes
 */

var PesquisaItemNaoConformidade = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", issue: 586, getType: typesKnockout.string, val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação", val: ko.observable(""), options: Global.ObterOpcoesPesquisaBooleano("Ativo", "Inativo"), def: "" });
    this.Grupo = PropertyEntity({ val: ko.observable(EnumGrupoNC.Todos), options: EnumGrupoNC.obterOpcoesPesquisa(), def: EnumGrupoNC.Todos, text: "Grupo: " });
    this.SubGrupo = PropertyEntity({ val: ko.observable(EnumSubGrupoNC.Todos), options: EnumSubGrupoNC.obterOpcoesPesquisa(), def: EnumSubGrupoNC.Todos, text: "SubGrupo: " });
    this.Area = PropertyEntity({ val: ko.observable(EnumAreaGrupoNC.Todos), options: EnumAreaGrupoNC.obterOpcoesPesquisa(), def: EnumAreaGrupoNC.Todos, text: "Área: " });
    this.IrrelevanteNaoConformidade = PropertyEntity({ val: ko.observable(EnumSimNaoPesquisa.Todos), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), def: EnumSimNaoPesquisa.Todos, text: "Irrelevante para Não Conformidade: " });
    this.PermiteContingencia = PropertyEntity({ val: ko.observable(EnumSimNaoPesquisa.Todos), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), def: EnumSimNaoPesquisa.Todos, text: "Permite Contigência: " });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridItemNaoConformidade.CarregarGrid();
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

function LoadItemNaoConformidade() {
    _pesquisaItemNaoConformidade = new PesquisaItemNaoConformidade();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridItemNaoConformidade = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ItemNaoConformidade/Pesquisa", _pesquisaItemNaoConformidade);

    _gridItemNaoConformidade.SetPermitirEdicaoColunas(true);
    _gridItemNaoConformidade.SetQuantidadeLinhasPorPagina(10);

    _relatorioItemNaoConformidade = new RelatorioGlobal("Relatorios/ItemNaoConformidade/BuscarDadosRelatorio", _gridItemNaoConformidade, function () {
        _relatorioItemNaoConformidade.loadRelatorio(function () {
            KoBindings(_pesquisaItemNaoConformidade, "knockoutPesquisaItemNaoConformidade", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaItemNaoConformidade", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaItemNaoConformidade", false);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaItemNaoConformidade);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function GerarRelatorioPDFClick() {
    _relatorioItemNaoConformidade.gerarRelatorio("Relatorios/ItemNaoConformidade/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick() {
    _relatorioItemNaoConformidade.gerarRelatorio("Relatorios/ItemNaoConformidade/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
