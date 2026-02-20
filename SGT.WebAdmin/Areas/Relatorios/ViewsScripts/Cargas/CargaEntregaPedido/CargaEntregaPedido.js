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
/// <reference path="../../../../../js/app.config.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridCargaEntregaPedido, _pesquisaCargaEntregaPedido, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioCargaEntregaPedido;

var _tipoDestino = [
    { text: "Todos", value: "T" },
    { text: "Coleta", value: "C" },
    { text: "Entrega", value: "E" }
];

var PesquisaCargaEntregaPedido = function () {
    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Transportadora = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(""), defCodEntity: "", text: "Transportadora:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.CargaAgendada = PropertyEntity({ val: ko.observable("Todos"), options: Global.ObterOpcoesPesquisaBooleano("Sim", "Não"), def: "Todos", text: "Carga Agendada:" });
    this.TipoDestino = PropertyEntity({ val: ko.observable("T"), options: _tipoDestino, def: "T", text: "Tipo Destino:" });
    this.SituacaoCarga = PropertyEntity({ getType: typesKnockout.selectMultiple, val: ko.observable(0), options: EnumSituacoesCarga.obterOpcoesEmbarcador(), def: 0, text: "Situação da Carga: ", issue: 533 });
    this.PossuiRedespacho = PropertyEntity({ text: "Possui Redespacho:", options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), val: ko.observable(EnumSimNaoPesquisa.Todos), def: EnumSimNaoPesquisa.Todos });

    this.DataCriacaoInicial = PropertyEntity({ text: "Inicial:", getType: typesKnockout.date, codEntity: ko.observable(""), idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataCriacaoFinal = PropertyEntity({ text: "Final:", getType: typesKnockout.date, dateRangeInit: this.DataCriacaoInicial, codEntity: ko.observable(""), idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataEntregaInicial = PropertyEntity({ text: "Inicial:", getType: typesKnockout.date, codEntity: ko.observable(""), idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataEntregaFinal = PropertyEntity({ text: "Final:", getType: typesKnockout.date, dateRangeInit: this.DataEntregaInicial, codEntity: ko.observable(""), idBtnSearch: guid(), visible: ko.observable(true) });
    this.PrevisaoEntregaPlanejadaInicio = PropertyEntity({ text: "Inícial:", getType: typesKnockout.date, dateRangeInit: this.DataEntregaInicial, codEntity: ko.observable(""), idBtnSearch: guid(), visible: ko.observable(true) });
    this.PrevisaoEntregaPlanejadaFinal = PropertyEntity({ text: "Final:", getType: typesKnockout.date, dateRangeInit: this.DataEntregaInicial, codEntity: ko.observable(""), idBtnSearch: guid(), visible: ko.observable(true) });
    this.MonitoramentoStatus = PropertyEntity({ text: "Situação do Monitoramento", getType: typesKnockout.selectMultiple, val: ko.observableArray([]), options: EnumMonitoramentoStatus.obterOpcoes(), def: [], visible: ko.observable(true) });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridCargaEntregaPedido.CarregarGrid();
        }, type: types.event, text: "Visualizar Prévia", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function LoadCargaEntregaPedido() {
    _pesquisaCargaEntregaPedido = new PesquisaCargaEntregaPedido();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridCargaEntregaPedido = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/CargaEntregaPedido/Pesquisa", _pesquisaCargaEntregaPedido);
    _gridCargaEntregaPedido.SetSalvarPreferenciasGrid(true);
    _gridCargaEntregaPedido.SetPermitirEdicaoColunas(true);
    _gridCargaEntregaPedido.SetQuantidadeLinhasPorPagina(10);

    _relatorioCargaEntregaPedido = new RelatorioGlobal("Relatorios/CargaEntregaPedido/BuscarDadosRelatorio", _gridCargaEntregaPedido, function () {
        _relatorioCargaEntregaPedido.loadRelatorio(function () {
            KoBindings(_pesquisaCargaEntregaPedido, "knockoutPesquisaCargaEntregaPedido", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaCargaEntregaPedido", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaCargaEntregaPedido", false);

            new BuscarFilial(_pesquisaCargaEntregaPedido.Filial);
            new BuscarTransportadores(_pesquisaCargaEntregaPedido.Transportadora);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaCargaEntregaPedido);

}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioCargaEntregaPedido.gerarRelatorio("Relatorios/CargaEntregaPedido/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioCargaEntregaPedido.gerarRelatorio("Relatorios/CargaEntregaPedido/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}

//*******MÉTODOS*******