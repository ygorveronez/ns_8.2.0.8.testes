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
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumEntradaSaida.js"/>
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumStatusNFe.js"/>
/// <reference path="../../Relatorios/Global/Relatorio.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridArmazenagem, _pesquisaArmazenagem, _CRUDRelatorio, _relatorioArmazenagem, _CRUDFiltrosRelatorio;

var _pesquisaStatusNF = [
    { text: "Todos", value: "" },
    { text: "Autorizada", value: EnumStatusNFe.Autorizado },
    { text: "Cancelada", value: EnumStatusNFe.Cancelado }
];

var PesquisaArmazenagem = function () {
    this.DataInicial = PropertyEntity({ getType: typesKnockout.date, text: "Data Inicial", val: ko.observable("") });
    this.DataFinal = PropertyEntity({ getType: typesKnockout.date, text: "Data Final", val: ko.observable("") });
    this.Tipo = PropertyEntity({ val: ko.observable(0), options: EnumEntradaSaida.obterOpcoesPesquisa(), def: 0, text: "Tipo: " });
    this.StatusNF = PropertyEntity({ val: ko.observable(0), options: _pesquisaStatusNF, def: 0, text: "Status NF: " });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridArmazenagem.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******

function loadRelatorioArmazenagem() {
    _pesquisaArmazenagem = new PesquisaArmazenagem();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridArmazenagem = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "/Relatorios/Armazenagem/Pesquisa", _pesquisaArmazenagem, null, null, 10);
    _gridArmazenagem.SetPermitirEdicaoColunas(true);

    _relatorioArmazenagem = new RelatorioGlobal("Relatorios/Armazenagem/BuscarDadosRelatorio", _gridArmazenagem, function () {
        _relatorioArmazenagem.loadRelatorio(function () {
            KoBindings(_pesquisaArmazenagem, "knockoutPesquisaArmazenagem");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaArmazenagem");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaArmazenagem");
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaArmazenagem);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioArmazenagem.gerarRelatorio("Relatorios/Armazenagem/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioArmazenagem.gerarRelatorio("Relatorios/Armazenagem/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
