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
/// <reference path="../../../../../ViewsScripts/Consultas/TipoDespesaFinanceira.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioDespesaMensal, _gridDespesaMensal, _pesquisaDespesaMensal, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaDespesaMensal = function () {
    this.DataInicialGeracao = PropertyEntity({ text: "Data Inicial Geração:", getType: typesKnockout.date });
    this.DataFinalGeracao = PropertyEntity({ text: "Data Final Geração:", getType: typesKnockout.date });
    this.DataInicialPagamento = PropertyEntity({ text: "Data Inicial Pagamento:", getType: typesKnockout.date });
    this.DataFinalPagamento = PropertyEntity({ text: "Data Final Pagamento:", getType: typesKnockout.date });

    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa: ", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoDespesa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Despesa: ", idBtnSearch: guid(), visible: ko.observable(true) });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridDespesaMensal.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******


function loadRelatorioDespesaMensal() {

    _pesquisaDespesaMensal = new PesquisaDespesaMensal();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridDespesaMensal = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/DespesaMensal/Pesquisa", _pesquisaDespesaMensal, null, null, 10);
    _gridDespesaMensal.SetPermitirEdicaoColunas(true);

    _relatorioDespesaMensal = new RelatorioGlobal("Relatorios/DespesaMensal/BuscarDadosRelatorio", _gridDespesaMensal, function () {
        _relatorioDespesaMensal.loadRelatorio(function () {
            KoBindings(_pesquisaDespesaMensal, "knockoutPesquisaDespesaMensal");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaDespesaMensal");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaDespesaMensal");

            new BuscarTipoDespesaFinanceira(_pesquisaDespesaMensal.TipoDespesa);
            new BuscarClientes(_pesquisaDespesaMensal.Pessoa);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaDespesaMensal);

}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioDespesaMensal.gerarRelatorio("Relatorios/DespesaMensal/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioDespesaMensal.gerarRelatorio("Relatorios/DespesaMensal/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}