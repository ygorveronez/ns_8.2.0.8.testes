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
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTransportadores, _pesquisaTransportadores, _CRUDRelatorio, _CRUDFiltrosRelatorio, _relatorioTransportadores;

var PesquisaTransportadores = function () {
    this.Localidade = PropertyEntity({ text: Localization.Resources.Transportadores.Transportadores.Localidade.getFieldDescription(), issue: 16, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Estado = PropertyEntity({ text: Localization.Resources.Transportadores.Transportadores.Estado.getFieldDescription(), issue: 12, type: types.entity, codEntity: ko.observable(""), defCodEntity: "", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Status = PropertyEntity({ text: Localization.Resources.Transportadores.Transportadores.Situacao.getFieldDescription(), issue: 556, options: _statusPesquisa, val: ko.observable(0), def: 0, visible: ko.observable(true) });
    this.PrazoValidade = PropertyEntity({ text: Localization.Resources.Transportadores.Transportadores.PrazoValidadeCertificado.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.EmiteEmbarcador = PropertyEntity({ text: Localization.Resources.Transportadores.Transportadores.EmiteEmbarcador.getFieldDescription(), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), val: ko.observable(EnumSimNaoPesquisa.Todos), def: EnumSimNaoPesquisa.Todos, visible: ko.observable(true) });
    this.CertificadosVencidos = PropertyEntity({ text: Localization.Resources.Transportadores.Transportadores.CertificadosVencidos, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.OptanteSimplesNacional = PropertyEntity({ text: Localization.Resources.Transportadores.Transportadores.OptanteSimplesNacional.getFieldDescription(), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), val: ko.observable(EnumSimNaoPesquisa.Todos), def: EnumSimNaoPesquisa.Todos, visible: ko.observable(true) });
    this.LiberacaoParaPagamentoAutomatico = PropertyEntity({ val: ko.observable(EnumSimNaoPesquisa.Todos), def: EnumSimNaoPesquisa.Todos, options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), text: Localization.Resources.Transportadores.Transportadores.IntegracaoAutomaticaCte.getFieldDescription(), visible: _CONFIGURACAO_TMS.PermitirAutomatizarPagamentoTransportador });
    this.ConfiguracaoNFSe = PropertyEntity({ text: Localization.Resources.Transportadores.Transportadores.ConfiguracaoNFSe.getFieldDescription(), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), val: ko.observable(EnumSimNaoPesquisa.Todos), def: EnumSimNaoPesquisa.Todos });
    this.Bloqueado = PropertyEntity({ options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), text: Localization.Resources.Transportadores.Transportadores.Bloqueado.getFieldDescription(), val: ko.observable(EnumSimNaoPesquisa.Todos) });
    this.DataInicioVencimentoCertificado = PropertyEntity({ text: Localization.Resources.Transportadores.Transportadores.DataInicioVencimentoCertificado.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinalVencimentoCertificado = PropertyEntity({ text: Localization.Resources.Transportadores.Transportadores.DataFinalVencimentoCertificado.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });

    this.DataCriacaoInicial = PropertyEntity({ text: Localization.Resources.Transportadores.Transportadores.DataCriacaoInicial.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataCriacaoFinal = PropertyEntity({ text: Localization.Resources.Transportadores.Transportadores.DataCriacaoFinal.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataAlteracaoInicial = PropertyEntity({ text: Localization.Resources.Transportadores.Transportadores.DataAlteracaoInicial.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataAlteracaoFinal = PropertyEntity({ text: Localization.Resources.Transportadores.Transportadores.DataAlteracaoFinal.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Transportadores.Transportadores.TipoRelatorio.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridTransportadores.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Transportadores.Transportadores.Preview, idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: Localization.Resources.Transportadores.Transportadores.GerarPDF });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: Localization.Resources.Transportadores.Transportadores.GerarPlanilhaExcel, idGrid: guid() });
};

//*******EVENTOS*******

function loadRelatorioTransportadores() {
    _pesquisaTransportadores = new PesquisaTransportadores();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridTransportadores = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/Transportador/Pesquisa", _pesquisaTransportadores);

    _gridTransportadores.SetPermitirEdicaoColunas(true);
    _gridTransportadores.SetQuantidadeLinhasPorPagina(25);

    _relatorioTransportadores = new RelatorioGlobal("Relatorios/Transportador/BuscarDadosRelatorio", _gridTransportadores, function () {
        _relatorioTransportadores.loadRelatorio(function () {
            KoBindings(_pesquisaTransportadores, "knockoutPesquisaTransportadores", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaTransportadores", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaTransportadores", false);

            BuscarLocalidades(_pesquisaTransportadores.Localidade);
            BuscarEstados(_pesquisaTransportadores.Estado);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaTransportadores);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioTransportadores.gerarRelatorio("Relatorios/Transportador/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioTransportadores.gerarRelatorio("Relatorios/Transportador/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}