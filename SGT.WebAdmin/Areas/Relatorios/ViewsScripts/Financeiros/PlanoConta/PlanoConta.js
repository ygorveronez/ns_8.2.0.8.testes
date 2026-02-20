/// <reference path="../../../../../ViewsScripts/Consultas/PlanoConta.js" />
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
//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioPlanoConta, _gridPlanoConta, _pesquisaPlanoConta, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _AnaliticoSintetico = [
    { text: "Todos", value: 0 },
    { text: "Analitico", value: EnumAnaliticoSintetico.Analitico },
    { text: "Sintético", value: EnumAnaliticoSintetico.Sintetico }
];

var PesquisaPlanoConta = function () {   
    this.PlanoSintetico = PropertyEntity({ text: "Plano de Contas Sintético:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Tipo = PropertyEntity({ val: ko.observable(0), options: _AnaliticoSintetico, text: "Tipo Conta: ", def: 0});

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridPlanoConta.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******

function loadRelatorioPlanoConta() {

    _pesquisaPlanoConta = new PesquisaPlanoConta();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridPlanoConta = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/PlanoConta/Pesquisa", _pesquisaPlanoConta, null, null, 10);
    _gridPlanoConta.SetPermitirEdicaoColunas(true);

    _relatorioPlanoConta = new RelatorioGlobal("Relatorios/PlanoConta/BuscarDadosRelatorio", _gridPlanoConta, function () {
        _relatorioPlanoConta.loadRelatorio(function () {
            KoBindings(_pesquisaPlanoConta, "knockoutPesquisaPlanoConta");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaPlanoConta");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaPlanoConta");

            new BuscarPlanoConta(_pesquisaPlanoConta.PlanoSintetico, "Selecione a Conta Sintética", "Contas Sintéticas", null, EnumAnaliticoSintetico.Sintetico);

            $("#divConteudoRelatorio").show();
        })
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaPlanoConta);

}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioPlanoConta.gerarRelatorio("Relatorios/PlanoConta/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioPlanoConta.gerarRelatorio("Relatorios/PlanoConta/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
