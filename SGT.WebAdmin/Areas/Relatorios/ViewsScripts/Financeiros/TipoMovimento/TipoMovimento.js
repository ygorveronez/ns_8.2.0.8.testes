/// <reference path="../../../../../ViewsScripts/Consultas/TipoMovimento.js" />
/// <reference path="../../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../../js/Global/CRUD.js" />
/// <reference path="../../../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../../../js/Global/Rest.js" />
/// <reference path="../../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../../js/Global/Grid.js" />
/// <reference path="../../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Localidade.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumAnaliticoSintetico.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/PlanoConta.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CentroResultado.js" />
//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioTipoMovimento, _gridTipoMovimento, _pesquisaTipoMovimento, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaTipoMovimento = function () {
    this.PlanoCredito = PropertyEntity({ text: "Conta gerencial de saída:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.PlanoDebito = PropertyEntity({ text: "Conta gerencial de entrada:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.CentroResultado = PropertyEntity({ text: "Centro de resultado:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });    

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridTipoMovimento.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******

function loadRelatorioTipoMovimento() {

    _pesquisaTipoMovimento = new PesquisaTipoMovimento();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridTipoMovimento = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/TipoMovimento/Pesquisa", _pesquisaTipoMovimento, null, null, 10);
    _gridTipoMovimento.SetPermitirEdicaoColunas(true);

    _relatorioTipoMovimento = new RelatorioGlobal("Relatorios/TipoMovimento/BuscarDadosRelatorio", _gridTipoMovimento, function () {
        _relatorioTipoMovimento.loadRelatorio(function () {
            KoBindings(_pesquisaTipoMovimento, "knockoutPesquisaTipoMovimento");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaTipoMovimento");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaTipoMovimento");

            new BuscarCentroResultado(_pesquisaTipoMovimento.CentroResultado, null, null, null, EnumAnaliticoSintetico.Analitico);
            new BuscarPlanoConta(_pesquisaTipoMovimento.PlanoDebito, "Selecione a Conta Analítica de Entrada", "Contas Gerencias", null, EnumAnaliticoSintetico.Analitico);
            new BuscarPlanoConta(_pesquisaTipoMovimento.PlanoCredito, "Selecione a Conta Analítica de Saída", "Contas Gerencias", null, EnumAnaliticoSintetico.Analitico);

            $("#divConteudoRelatorio").show();
        })
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaTipoMovimento);

}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioTipoMovimento.gerarRelatorio("Relatorios/TipoMovimento/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioTipoMovimento.gerarRelatorio("Relatorios/TipoMovimento/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
