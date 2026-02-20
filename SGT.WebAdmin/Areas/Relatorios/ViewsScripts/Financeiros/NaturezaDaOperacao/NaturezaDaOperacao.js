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
/// <reference path="../../../../../ViewsScripts/Consultas/NaturezaOperacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioNaturezaDaOperacao, _gridNaturezaDaOperacao, _pesquisaNaturezaDaOperacao, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaNaturezaDaOperacao = function () {
    this.NaturezaDaOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Natureza da Operação:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridExtratoConta.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: guid(), visible: ko.observable(true)
    });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridNaturezaDaOperacao.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******

function loadRelatorioNaturezaDaOperacao() {

    _pesquisaNaturezaDaOperacao = new PesquisaNaturezaDaOperacao();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridNaturezaDaOperacao = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/NaturezaDaOperacao/Pesquisa", _pesquisaNaturezaDaOperacao, null, null, 10);
    _gridNaturezaDaOperacao.SetPermitirEdicaoColunas(true);

    _relatorioNaturezaDaOperacao = new RelatorioGlobal("Relatorios/NaturezaDaOperacao/BuscarDadosRelatorio", _gridNaturezaDaOperacao, function () {
        _relatorioNaturezaDaOperacao.loadRelatorio(function () {
            KoBindings(_pesquisaNaturezaDaOperacao, "knockoutPesquisaNaturezaDaOperacao");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaNaturezaDaOperacao");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaNaturezaDaOperacao");

            new BuscarNaturezasOperacoesNotaFiscal(_pesquisaNaturezaDaOperacao.NaturezaDaOperacao);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaNaturezaDaOperacao);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioNaturezaDaOperacao.gerarRelatorio("Relatorios/NaturezaDaOperacao/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioNaturezaDaOperacao.gerarRelatorio("Relatorios/NaturezaDaOperacao/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
