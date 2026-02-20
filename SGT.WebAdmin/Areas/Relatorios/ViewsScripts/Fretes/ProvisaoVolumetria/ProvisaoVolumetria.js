/// <reference path="../../../../../js/Global/CRUD.js" />
/// <reference path="../../../../../js/Global/Globais.js" />
/// <reference path="../../../../../js/Global/Grid.js" />
/// <reference path="../../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../../js/Global/Rest.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoOperacao.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioProvisaoVolumetria;
var _pesquisaProvisaoVolumetria;
var _gridProvisaoVolumetria;
var _CRUDRelatorio;
var _CRUDFiltrosRelatorio;

var PesquisaProvisaoVolumetria = function () {
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Mercado = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Mercado:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.DataEmissaoNFInicio = PropertyEntity({ text: ko.observable("*Data Emissão NF Inicial: "), val: ko.observable(Global.DataAtual()), def: Global.DataAtual(), getType: typesKnockout.date, visible: ko.observable(true), required: ko.observable(true) });
    this.DataEmissaoNFFim = PropertyEntity({ text: ko.observable("*Data Emissão NF Final: "), val: ko.observable(Global.DataAtual()), def: Global.DataAtual(), getType: typesKnockout.date, visible: ko.observable(true), required: ko.observable(true) });
    this.DataEmissaoNFInicio.dateRangeLimit = this.DataEmissaoNFFim;
    this.DataEmissaoNFFim.dateRangeInit = this.DataEmissaoNFInicio;

    this.DataInicialEmissaoCTe = PropertyEntity({ text: "Data Emissão Carga Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinalEmissaoCTe = PropertyEntity({ text: "Data Emissão Carga Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicialEmissaoCTe.dateRangeLimit = this.DataFinalEmissaoCTe;
    this.DataFinalEmissaoCTe.dateRangeInit = this.DataInicialEmissaoCTe;

    this.DataIntegracaoPagamentoInicio = PropertyEntity({ text: "Data Integração Pagamento Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataIntegracaoPagamentoFim = PropertyEntity({ text: "Data Integração Pagamento Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataIntegracaoPagamentoInicio.dateRangeLimit = this.DataIntegracaoPagamentoFim;
    this.DataIntegracaoPagamentoFim.dateRangeInit = this.DataIntegracaoPagamentoInicio;

    this.DataInicialPrevisaoEntregaPedido = PropertyEntity({ text: "Data Previsão Entrega Pedido Inicial:", val: ko.observable(""), def: "", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataFinalPrevisaoEntregaPedido = PropertyEntity({ text: "Data Previsão Entrega Pedido Final:", val: ko.observable(""), def: "", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataInicialPrevisaoEntregaPedido.dateRangeLimit = this.DataFinalPrevisaoEntregaPedido;
    this.DataFinalPrevisaoEntregaPedido.dateRangeInit = this.DataInicialPrevisaoEntregaPedido;

    this.DataVencimentoInicio = PropertyEntity({ text: "Data Vencimento Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataVencimentoFim = PropertyEntity({ text: "Data Vencimento Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataVencimentoInicio.dateRangeLimit = this.DataVencimentoFim;
    this.DataVencimentoFim.dateRangeInit = this.DataVencimentoInicio;

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridProvisaoVolumetria.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function loadProvisaoVolumetria() {
    _pesquisaProvisaoVolumetria = new PesquisaProvisaoVolumetria();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridProvisaoVolumetria = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ProvisaoVolumetria/Pesquisa", _pesquisaProvisaoVolumetria, null, null, 10);
    _gridProvisaoVolumetria.SetPermitirEdicaoColunas(true);
    _gridProvisaoVolumetria.SetHabilitarScrollHorizontal(true, 200);

    _relatorioProvisaoVolumetria = new RelatorioGlobal("Relatorios/ProvisaoVolumetria/BuscarDadosRelatorio", _gridProvisaoVolumetria, function () {
        _relatorioProvisaoVolumetria.loadRelatorio(function () {
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaProvisaoVolumetria");
            KoBindings(_pesquisaProvisaoVolumetria, "knockoutPesquisaProvisaoVolumetria", false, _CRUDFiltrosRelatorio.Preview.id);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaProvisaoVolumetria");

            new BuscarTransportadores(_pesquisaProvisaoVolumetria.Transportador);
            new BuscarTiposOperacao(_pesquisaProvisaoVolumetria.TipoOperacao);
            new BuscarFilial(_pesquisaProvisaoVolumetria.Filial);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaProvisaoVolumetria);

}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioProvisaoVolumetria.gerarRelatorio("Relatorios/ProvisaoVolumetria/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioProvisaoVolumetria.gerarRelatorio("Relatorios/ProvisaoVolumetria/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
