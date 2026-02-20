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
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../js/app.config.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridRegraICMS, _pesquisaRegraICMS, _CRUDRelatorio, _CRUDFiltrosRelatorio;
var _relatorioRegraICMS;

var PesquisaRegraICMS = function () {

    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.DataInicio = PropertyEntity({ text: "Data Vigência Início: ", getType: typesKnockout.date });
    this.DataFim = PropertyEntity({ text: "Data Vigência Fim: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.Ativo = PropertyEntity({ val: ko.observable(0), options: _statusPesquisa, def: 0, text: "Situação: " });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridRegraICMS.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Preview, idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaRegraICMS.Visible.visibleFade()) {
                _pesquisaRegraICMS.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaRegraICMS.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Avancada, icon: ko.observable("fal fa-plus"), visible: ko.observable(false)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: Localization.Resources.Gerais.Geral.GerarPDF });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: Localization.Resources.Gerais.Geral.GerarPlanilhaExcel, idGrid: guid() });
};

//*******EVENTOS*******

function LoadRegraICMS() {
    _pesquisaRegraICMS = new PesquisaRegraICMS();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridRegraICMS = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/RegraICMS/Pesquisa", _pesquisaRegraICMS);

    _gridRegraICMS.SetPermitirEdicaoColunas(true);
    _gridRegraICMS.SetQuantidadeLinhasPorPagina(10);

    _relatorioRegraICMS = new RelatorioGlobal("Relatorios/RegraICMS/BuscarDadosRelatorio", _gridRegraICMS, function () {
        _relatorioRegraICMS.loadRelatorio(function () {
            KoBindings(_pesquisaRegraICMS, "knockoutPesquisaRegraICMS", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaRegraICMS", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaRegraICMS", false);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaRegraICMS);

    controlarCampos();
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioRegraICMS.gerarRelatorio("Relatorios/RegraICMS/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioRegraICMS.gerarRelatorio("Relatorios/RegraICMS/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}

//*******MÉTODOS*******

function controlarCampos() {
    //_pesquisaRegraICMS.Proprietario.visible(_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.Terceiros);
    //_pesquisaRegraICMS.Transportador.visible(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador);
    //_pesquisaRegraICMS.CentroCarregamento.visible(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador);
}