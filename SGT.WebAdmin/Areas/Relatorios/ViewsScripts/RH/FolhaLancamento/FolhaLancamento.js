/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/FolhaInformacao.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/StatusUsuario.js" />

//*******MAPEAMENTO KNOUCKOUT*******

let _relatorioFolhaLancamento, _gridFolhaLancamento, _pesquisaFolhaLancamento, _CRUDRelatorio, _CRUDFiltrosRelatorio;

const PesquisaFolhaLancamento = function () {
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report })

    this.Funcionario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Funcionário:", idBtnSearch: guid(), visible: true });
    this.InformacaoFolha = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Informação Folha:", idBtnSearch: guid(), visible: true });
    this.DataInicial = PropertyEntity({ getType: typesKnockout.date, text: "Data Inicial:", visible: true });
    this.DataFinal = PropertyEntity({ getType: typesKnockout.date, text: "Data Final:", visible: true });
    this.DataCompetenciaInicial = PropertyEntity({ getType: typesKnockout.date, text: "Data Inicial de Competência:", visible: true });
    this.DataCompetenciaFinal = PropertyEntity({ getType: typesKnockout.date, text: "Data Final de Competência:", visible: true });
    this.SituacaoFuncionario = PropertyEntity({ getType: typesKnockout.select, val: ko.observable(EnumStatusUsuario.Ativo), def: EnumStatusUsuario.Ativo, options: EnumStatusUsuario.obterOpcoesPesquisa(), text: "Situação:" });
};

const CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridFolhaLancamento.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

const CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function LoadFolhaLancamento() {
    _pesquisaFolhaLancamento = new PesquisaFolhaLancamento();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridFolhaLancamento = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/FolhaLancamento/Pesquisar", _pesquisaFolhaLancamento, null, null, 10);
    _gridFolhaLancamento.SetPermitirEdicaoColunas(true);

    _relatorioFolhaLancamento = new RelatorioGlobal("Relatorios/FolhaLancamento/BuscarDadosRelatorio", _gridFolhaLancamento, function () {
        _relatorioFolhaLancamento.loadRelatorio(function () {
            KoBindings(_pesquisaFolhaLancamento, "knockoutPesquisaFolhaLancamento", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaFolhaLancamento", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaFolhaLancamento", false);

            BuscarFuncionario(_pesquisaFolhaLancamento.Funcionario);
            BuscarFolhaInformacao(_pesquisaFolhaLancamento.InformacaoFolha);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaFolhaLancamento);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioFolhaLancamento.gerarRelatorio("Relatorios/FolhaLancamento/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioFolhaLancamento.gerarRelatorio("Relatorios/FolhaLancamento/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
