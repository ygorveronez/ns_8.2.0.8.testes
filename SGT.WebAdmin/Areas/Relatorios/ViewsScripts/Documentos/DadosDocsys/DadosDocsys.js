/// <reference path="../../../../../ViewsScripts/Consultas/PedidoViagemNavio.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridDadosDocsys, _pesquisaDadosDocsys, _CRUDRelatorio, _relatorioDadosDocsys, _CRUDFiltrosRelatorio;

var PesquisaDadosDocsys = function () {
    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });
    this.PedidoViagemNavio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Viagem:", idBtnSearch: guid() });
    this.DataEmissaoInicial = PropertyEntity({ text: "Data Emissão Inicial: ", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.DataEmissaoFinal = PropertyEntity({ text: "Data Emissão Final: ", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });

    this.DataEmissaoInicial.dateRangeLimit = this.DataEmissaoFinal;
    this.DataEmissaoFinal.dateRangeInit = this.DataEmissaoInicial;
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridDadosDocsys.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaDadosDocsys.Visible.visibleFade() == true) {
                _pesquisaDadosDocsys.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaDadosDocsys.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******

function LoadDadosDocsys() {

    _pesquisaDadosDocsys = new PesquisaDadosDocsys();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridDadosDocsys = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "/Relatorios/DadosDocsys/Pesquisa", _pesquisaDadosDocsys, null, null, 10, null, null, null, null, 20);
    _gridDadosDocsys.SetPermitirEdicaoColunas(true);

    _relatorioDadosDocsys = new RelatorioGlobal("Relatorios/DadosDocsys/BuscarDadosRelatorio", _gridDadosDocsys, function () {
        _relatorioDadosDocsys.loadRelatorio(function () {
            KoBindings(_pesquisaDadosDocsys, "knockoutPesquisaDadosDocsys");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaDadosDocsys");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaDadosDocsys");

            new BuscarPedidoViagemNavio(_pesquisaDadosDocsys.PedidoViagemNavio);
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaDadosDocsys);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioDadosDocsys.gerarRelatorio("Relatorios/DadosDocsys/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioDadosDocsys.gerarRelatorio("Relatorios/DadosDocsys/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
