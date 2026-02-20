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
/// <reference path="../../../../../ViewsScripts/Consultas/TipoPagamentoRecebimento.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoTitulo.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioBaixaTitulo, _gridBaixaTitulo, _pesquisaBaixaTitulo, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _tipoTitulo = [
    { text: "A Receber", value: EnumTipoTitulo.AReceber },
    { text: "A Pagar", value: EnumTipoTitulo.APagar }
];

var PesquisaBaixaTitulo = function () {
    this.DataInicial = PropertyEntity({ text: "Data Baixa Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Baixa Final: ", getType: typesKnockout.date });

    this.TipoTitulo = PropertyEntity({ val: ko.observable(EnumTipoTitulo.AReceber), options: _tipoTitulo, def: EnumTipoTitulo.AReceber, text: "Tipo Título:" });
    this.TipoPagamentoRecebimento = PropertyEntity({ text: "Tipo Pagamento Recebimento:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), val: ko.observable("") });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridBaixaTitulo.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******


function loadRelatorioBaixaTitulo() {

    _pesquisaBaixaTitulo = new PesquisaBaixaTitulo();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridBaixaTitulo = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/BaixaTitulo/Pesquisa", _pesquisaBaixaTitulo, null, null, 10);
    _gridBaixaTitulo.SetPermitirEdicaoColunas(true);

    _relatorioBaixaTitulo = new RelatorioGlobal("Relatorios/BaixaTitulo/BuscarDadosRelatorio", _gridBaixaTitulo, function () {
        _relatorioBaixaTitulo.loadRelatorio(function () {
            KoBindings(_pesquisaBaixaTitulo, "knockoutPesquisaBaixaTitulo");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaBaixaTitulo");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaBaixaTitulo");

            new BuscarTipoPagamentoRecebimento(_pesquisaBaixaTitulo.TipoPagamentoRecebimento);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaBaixaTitulo);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioBaixaTitulo.gerarRelatorio("Relatorios/BaixaTitulo/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioBaixaTitulo.gerarRelatorio("Relatorios/BaixaTitulo/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}