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
/// <reference path="../../../../../ViewsScripts/Consultas/CFOP.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoCFOP.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumStatusCFOP.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioCFOP, _gridCFOP, _pesquisaCFOP, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaCFOP = function () {
    this.CFOP = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "CFOP:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Extensao = PropertyEntity({ text: "Extensão:", getType: typesKnockout.string, val: ko.observable(""), def: "" });
    this.Descricao = PropertyEntity({ text: "Descrição:", getType: typesKnockout.string, val: ko.observable(""), def: "" });
    this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoCFOPPesquisa.Entrada), options: EnumTipoCFOPPesquisa.obterOpcoesPesquisa(), def: EnumTipoCFOPPesquisa.Entrada, text: "Tipo: " });
    this.Status = PropertyEntity({ val: ko.observable(EnumStatusCFOP.Ativo), options: EnumStatusCFOP.obterOpcoesPesquisa(), def: EnumStatusCFOP.Ativo, text: "Status: " });
    this.GerarEstoque = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Gera Estoque?", def: false });
    this.RealizaRateioDespesaVeiculo = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Realiza o rateio para a despesa do veículo?", def: false });

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
            _gridCFOP.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******

function loadRelatorioCFOP() {

    _pesquisaCFOP = new PesquisaCFOP();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridCFOP = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/CFOP/Pesquisa", _pesquisaCFOP, null, null, 10);
    _gridCFOP.SetPermitirEdicaoColunas(true);

    _relatorioCFOP = new RelatorioGlobal("Relatorios/CFOP/BuscarDadosRelatorio", _gridCFOP, function () {
        _relatorioCFOP.loadRelatorio(function () {
            KoBindings(_pesquisaCFOP, "knockoutPesquisaCFOP");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaCFOP");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaCFOP");

            new BuscarCFOPNotaFiscal(_pesquisaCFOP.CFOP);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaCFOP);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioCFOP.gerarRelatorio("Relatorios/CFOP/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioCFOP.gerarRelatorio("Relatorios/CFOP/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
