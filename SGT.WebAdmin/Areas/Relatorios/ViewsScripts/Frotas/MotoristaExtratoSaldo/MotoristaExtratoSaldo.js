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
/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioMotoristaExtratoSaldo, _gridMotoristaExtratoSaldo, _pesquisaMotoristaExtratoSaldo, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaMotoristaExtratoSaldo = function () {
    this.DataInicial = PropertyEntity({ text: "Período inicial: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });
    this.Motorista = PropertyEntity({ text: "Motorista:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridMotoristaExtratoSaldo.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: guid(), visible: ko.observable(true)
    });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridMotoristaExtratoSaldo.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******

function loadRelatorioMotoristaExtratoSaldo() {

    _pesquisaMotoristaExtratoSaldo = new PesquisaMotoristaExtratoSaldo();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridMotoristaExtratoSaldo = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/MotoristaExtratoSaldo/Pesquisa", _pesquisaMotoristaExtratoSaldo, null, null, 100);
    _gridMotoristaExtratoSaldo.SetPermitirEdicaoColunas(true);


    _relatorioMotoristaExtratoSaldo = new RelatorioGlobal("Relatorios/MotoristaExtratoSaldo/BuscarDadosRelatorio", _gridMotoristaExtratoSaldo, function () {
        _relatorioMotoristaExtratoSaldo.loadRelatorio(function () {
            KoBindings(_pesquisaMotoristaExtratoSaldo, "knockoutPesquisaMotoristaExtratoSaldo");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaMotoristaExtratoSaldo");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaMotoristaExtratoSaldo");

            new BuscarMotoristas(_pesquisaMotoristaExtratoSaldo.Motorista, retornoMotorista);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaMotoristaExtratoSaldo);

}

function retornoMotorista(data) {
    _pesquisaMotoristaExtratoSaldo.Motorista.codEntity(data.Codigo);
    _pesquisaMotoristaExtratoSaldo.Motorista.val(data.Nome);
}


function GerarRelatorioPDFClick(e, sender) {
    _relatorioMotoristaExtratoSaldo.gerarRelatorio("Relatorios/MotoristaExtratoSaldo/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioMotoristaExtratoSaldo.gerarRelatorio("Relatorios/MotoristaExtratoSaldo/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}