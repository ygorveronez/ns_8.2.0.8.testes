//#region Referências

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
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Empresa.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Localidade.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoCarga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoOperacao.js" />

//#endregion

var _pesquisaCondicoesPagamentoTransportador;
var _relatorioCondicoesPagamentoTransportador;
var _gridCondicoesPagamentoTransportador;
var _CRUDFiltrosRelatorio;
var _CRUDRelatorio;

var PesquisaCondicoesPagamentoTransportador = function () {
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de Integração:", val: ko.observable(""), def: "" });
    this.Estado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Estado:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Carga:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridCondicoesPagamentoTransportador.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaCondicoesPagamentoTransportador.Visible.visibleFade()) {
                _pesquisaCondicoesPagamentoTransportador.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaCondicoesPagamentoTransportador.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

function LoadCondicoesPagamentoTransportador() {
    _pesquisaCondicoesPagamentoTransportador = new PesquisaCondicoesPagamentoTransportador();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridCondicoesPagamentoTransportador = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/CondicoesPagamentoTransportador/Pesquisa", _pesquisaCondicoesPagamentoTransportador, null, null, 10);
    _gridCondicoesPagamentoTransportador.SetPermitirEdicaoColunas(true);

    _relatorioCondicoesPagamentoTransportador = new RelatorioGlobal("Relatorios/CondicoesPagamentoTransportador/BuscarDadosRelatorio", _gridCondicoesPagamentoTransportador, function () {
        _relatorioCondicoesPagamentoTransportador.loadRelatorio(function () {
            KoBindings(_pesquisaCondicoesPagamentoTransportador, "knockoutPesquisaCondicoesPagamentoTransportador");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaCondicoesPagamentoTransportador");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaCondicoesPagamentoTransportador");

            new BuscarTransportadores(_pesquisaCondicoesPagamentoTransportador.Empresa);
            new BuscarEstados(_pesquisaCondicoesPagamentoTransportador.Estado);
            new BuscarTiposdeCarga(_pesquisaCondicoesPagamentoTransportador.TipoCarga);
            new BuscarTiposOperacao(_pesquisaCondicoesPagamentoTransportador.TipoOperacao);
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaCondicoesPagamentoTransportador);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioCondicoesPagamentoTransportador.gerarRelatorio("Relatorios/CondicoesPagamentoTransportador/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioCondicoesPagamentoTransportador.gerarRelatorio("Relatorios/CondicoesPagamentoTransportador/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
