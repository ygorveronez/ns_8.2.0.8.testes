/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumEtapaAutorizacaoOcorrencia.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDRelatorio;
var _CRUDFiltrosRelatorio;
var _gridRegrasAutorizacaoOcorrencia;
var _pesquisaRegrasAutorizacaoOcorrencia;
var _relatorioRegrasAutorizacaoOcorrencia;

/*
 * Declaração das Classes
 */

var PesquisaRegrasAutorizacaoOcorrencia = function () {
    this.Aprovador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Aprovador:"), idBtnSearch: guid() });
    this.Ativo = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });
    this.DataVgenciaInicial = PropertyEntity({ text: "Início da Vigência: ", getType: typesKnockout.date });
    this.DataVgenciaLimite = PropertyEntity({ text: "Limite da Vigência: ", dateRangeInit: this.DataInicial, getType: typesKnockout.date });
    this.Descricao = PropertyEntity({ text: "Descrição:" });
    this.EtapaAutorizacao = PropertyEntity({ text: "Etapa de Autorização: ", val: ko.observable(EnumEtapaAutorizacaoOcorrencia.Todas), options: EnumEtapaAutorizacaoOcorrencia.obterOpcoesPesquisa(), def: EnumEtapaAutorizacaoOcorrencia.Todas });
    this.ExibirAlcadas = PropertyEntity({ text: "Exibir Detalhes?", val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.DataVgenciaInicial.dateRangeLimit = this.DataVgenciaLimite;
    this.DataVgenciaLimite.dateRangeInit = this.DataVgenciaInicial;

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridRegrasAutorizacaoOcorrencia.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

/*
 * Declaração das Funções de Inicialização
 */

function LoadRegrasAutorizacaoOcorrencia() {
    _pesquisaRegrasAutorizacaoOcorrencia = new PesquisaRegrasAutorizacaoOcorrencia();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();
    _gridRegrasAutorizacaoOcorrencia = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/RegrasAutorizacaoOcorrencia/Pesquisa", _pesquisaRegrasAutorizacaoOcorrencia);

    _gridRegrasAutorizacaoOcorrencia.SetPermitirEdicaoColunas(true);
    _gridRegrasAutorizacaoOcorrencia.SetQuantidadeLinhasPorPagina(20);

    _relatorioRegrasAutorizacaoOcorrencia = new RelatorioGlobal("Relatorios/RegrasAutorizacaoOcorrencia/BuscarDadosRelatorio", _gridRegrasAutorizacaoOcorrencia, function () {
        _relatorioRegrasAutorizacaoOcorrencia.loadRelatorio(function () {
            KoBindings(_pesquisaRegrasAutorizacaoOcorrencia, "knockoutPesquisaRegrasAutorizacaoOcorrencia", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaRegrasAutorizacaoOcorrencia", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaRegrasAutorizacaoOcorrencia", false);

            new BuscarFuncionario(_pesquisaRegrasAutorizacaoOcorrencia.Aprovador);

            var knoutRelatorio = _relatorioRegrasAutorizacaoOcorrencia.obterKnoutRelatorio();

            knoutRelatorio.AgruparRelatorio.visible(false);
            knoutRelatorio.AgruparRelatorio.val(false);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaRegrasAutorizacaoOcorrencia);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function GerarRelatorioPDFClick() {
    _relatorioRegrasAutorizacaoOcorrencia.gerarRelatorio("Relatorios/RegrasAutorizacaoOcorrencia/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick() {
    _relatorioRegrasAutorizacaoOcorrencia.gerarRelatorio("Relatorios/RegrasAutorizacaoOcorrencia/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}