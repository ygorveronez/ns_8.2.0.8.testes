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

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioAFRMMControl, _gridAFRMMControl, _pesquisaAFRMMControl, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaAFRMMControl = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", issue: 2, getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", issue: 2, getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridAFRMMControl.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
    this.GerarArquivoMercante = PropertyEntity({ eventClick: GerarArquivoMercanteClick, type: types.event, text: "Gerar Arquivo Interface", idGrid: guid() });
};

//*******EVENTOS*******


function loadRelatorioAFRMMControl() {

    _pesquisaAFRMMControl = new PesquisaAFRMMControl();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridAFRMMControl = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/AFRMMControl/Pesquisa", _pesquisaAFRMMControl, null, null, 10);
    _gridAFRMMControl.SetPermitirEdicaoColunas(true);

    _relatorioAFRMMControl = new RelatorioGlobal("Relatorios/AFRMMControl/BuscarDadosRelatorio", _gridAFRMMControl, function () {
        _relatorioAFRMMControl.loadRelatorio(function () {
            KoBindings(_pesquisaAFRMMControl, "knockoutPesquisaAFRMMControl");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaAFRMMControl");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaAFRMMControl");

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaAFRMMControl);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioAFRMMControl.gerarRelatorio("Relatorios/AFRMMControl/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioAFRMMControl.gerarRelatorio("Relatorios/AFRMMControl/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}

function GerarArquivoMercanteClick(e, sender) {
    _relatorioAFRMMControl.gerarRelatorio("Relatorios/AFRMMControl/GerarRelatorio", EnumTipoArquivoRelatorio.DOC);
}