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
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ContratoNotaFiscal.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoAceiteContrato.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioAceiteContrato, _gridAceiteContrato, _pesquisaAceiteContrato, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaAceiteContrato = function () {
    this.Situacao = PropertyEntity({ text: Localization.Resources.Transportadores.Transportadores.Situacao.getFieldDescription(), val: ko.observable(EnumSituacaoAceiteContrato.Todos), options: EnumSituacaoAceiteContrato.obterOpcoesPesquisa(), def: EnumSituacaoAceiteContrato.Todos });

    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Transportadores.Transportadores.Transportador.getFieldDescription(), idBtnSearch: guid() });
    this.ContratoNotaFiscal = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Transportadores.Transportadores.Contrato.getFieldDescription(), idBtnSearch: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Transportadores.Transportadores.TipoRelatorio.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridAceiteContrato.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Transportadores.Transportadores.Preview, idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: Localization.Resources.Transportadores.Transportadores.GerarPDF });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: Localization.Resources.Transportadores.Transportadores.GerarPlanilhaExcel, idGrid: guid() });
};

//*******EVENTOS*******

function loadRelatorioAceiteContrato() {

    _pesquisaAceiteContrato = new PesquisaAceiteContrato();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridAceiteContrato = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/AceiteContrato/Pesquisa", _pesquisaAceiteContrato, null, null, 10);
    _gridAceiteContrato.SetPermitirEdicaoColunas(true);

    _relatorioAceiteContrato = new RelatorioGlobal("Relatorios/AceiteContrato/BuscarDadosRelatorio", _gridAceiteContrato, function () {
        _relatorioAceiteContrato.loadRelatorio(function () {
            KoBindings(_pesquisaAceiteContrato, "knockoutPesquisaAceiteContrato");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaAceiteContrato");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaAceiteContrato");

            new BuscarTransportadores(_pesquisaAceiteContrato.Transportador);
            new BuscarContratoNotaFiscal(_pesquisaAceiteContrato.ContratoNotaFiscal);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaAceiteContrato);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioAceiteContrato.gerarRelatorio("Relatorios/AceiteContrato/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioAceiteContrato.gerarRelatorio("Relatorios/AceiteContrato/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}