/// <reference path="../../../../../ViewsScripts/Consultas/CentroResultado.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoPessoa.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoOperacao.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumStatusGestaoCarga.js" />
/// <reference path="../../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../../js/Global/CRUD.js" />
/// <reference path="../../../../../js/Global/Rest.js" />
/// <reference path="../../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../../js/Global/Grid.js" />
/// <reference path="../../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../js/app.config.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridGestaoCarga, _pesquisaGestaoCarga, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioGestaoCarga;

var PesquisaGestaoCarga = function () {
    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Cargas.GestaoCarga.TipoRelatorio, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.DataInicial = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.GestaoCarga.DataInicial.getFieldDescription(), issue: 2, getType: typesKnockout.date, required: ko.observable(false) });
    this.DataFinal = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.GestaoCarga.DataFinal.getFieldDescription(), issue: 2, getType: typesKnockout.date, required: ko.observable(false) });
    this.StatusGestaoCarga = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.GestaoCarga.Status.getFieldDescription(), val: ko.observable(EnumStatusGestaoCarga.Todas), options: EnumStatusGestaoCarga.obterOpcoesPesquisa(), def: EnumStatusGestaoCarga.Todas });

    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Relatorios.Cargas.GestaoCarga.TipoDeOperacao.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoa = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Gerais.Geral.GrupoPessoas.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(true) });
    this.CentroResultado = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Cargas.GestaoCarga.CentroResultado.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Tomador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Relatorios.Cargas.GestaoCarga.Tomador.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(true) });
    this.IncluirOperacoesDeslocamentoVazio = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.GestaoCarga.IncluirOperacoesDeDeslocamentoVazio, val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.NumeroCTe = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.GestaoCarga.NumeroCTe, getType: typesKnockout.int, visible: ko.observable(true) });
    this.NumeroNF = PropertyEntity({ text: Localization.Resources.Relatorios.Cargas.GestaoCarga.NumeroNF, getType: typesKnockout.int, visible: ko.observable(true) });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridGestaoCarga.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioGestao = PropertyEntity({ eventClick: GerarRelatorioGestaoClick, type: types.event, text: Localization.Resources.Relatorios.Cargas.GestaoCarga.PDFGestaoDeCarga });
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: Localization.Resources.Relatorios.Cargas.GestaoCarga.GerarPDF });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: Localization.Resources.Relatorios.Cargas.GestaoCarga.GerarPlanilhaExcel, idGrid: guid() });
};

//*******EVENTOS*******

function LoadGestaoCarga() {
    _pesquisaGestaoCarga = new PesquisaGestaoCarga();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridGestaoCarga = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/GestaoCarga/Pesquisa", _pesquisaGestaoCarga);

    _gridGestaoCarga.SetPermitirEdicaoColunas(true);
    _gridGestaoCarga.SetQuantidadeLinhasPorPagina(10);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.Fornecedor || _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.Terceiros || _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe) {
        _pesquisaGestaoCarga.TipoOperacao.visible(false);
        _pesquisaGestaoCarga.GrupoPessoa.visible(false);
        _pesquisaGestaoCarga.CentroResultado.visible(false);
        _pesquisaGestaoCarga.Tomador.visible(false);
    }

    _relatorioGestaoCarga = new RelatorioGlobal("Relatorios/GestaoCarga/BuscarDadosRelatorio", _gridGestaoCarga, function () {
        _relatorioGestaoCarga.loadRelatorio(function () {
            KoBindings(_pesquisaGestaoCarga, "knockoutPesquisaGestaoCarga", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaGestaoCarga", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaGestaoCarga", false);

            new BuscarTiposOperacao(_pesquisaGestaoCarga.TipoOperacao);
            new BuscarGruposPessoas(_pesquisaGestaoCarga.GrupoPessoa);
            new BuscarCentroResultado(_pesquisaGestaoCarga.CentroResultado);
            new BuscarClientes(_pesquisaGestaoCarga.Tomador);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaGestaoCarga);

}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioGestaoCarga.gerarRelatorio("Relatorios/GestaoCarga/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioGestaoCarga.gerarRelatorio("Relatorios/GestaoCarga/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}

function GerarRelatorioGestaoClick(e, sender) {

    var dados = {
        DataInicial: _pesquisaGestaoCarga.DataInicial,
        DataFinal: _pesquisaGestaoCarga.DataFinal,
        StatusGestaoCarga: _pesquisaGestaoCarga.StatusGestaoCarga,
        TipoOperacao: _pesquisaGestaoCarga.TipoOperacao,
        GrupoPessoa: _pesquisaGestaoCarga.GrupoPessoa,
        CentroResultado: _pesquisaGestaoCarga.CentroResultado,
        Tomador: _pesquisaGestaoCarga.Tomador,
        IncluirOperacoesDeslocamentoVazio: _pesquisaGestaoCarga.IncluirOperacoesDeslocamentoVazio,
    }

    executarDownload("Relatorios/GestaoCarga/GerarRelatorioGestao", RetornarObjetoPesquisa(dados));
}

//*******MÉTODOS*******