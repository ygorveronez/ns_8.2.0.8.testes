//*******MAPEAMENTO KNOUCKOUT*******

var _gridPedagio, _pesquisaPedagio, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioPedagio;

var _situacoesPedagio = [
    { text: "Inconsistente", value: EnumSituacaoPedagio.Inconsistente },
    { text: "Lançado", value: EnumSituacaoPedagio.Lancado },
    { text: "Fechado", value: EnumSituacaoPedagio.Fechado }
];
var _pesquisaTipoPedagio = [{ text: "Todos", value: EnumTipoPedagio.Todos }, { text: "Débito", value: EnumTipoPedagio.Debito }, { text: "Crédito", value: EnumTipoPedagio.Credito }];

var PesquisaPedagio = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.TipoPedagio = PropertyEntity({ text: "Tipo: ", val: ko.observable(EnumTipoPedagio.Todos), options: _pesquisaTipoPedagio, def: EnumTipoPedagio.Todos });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Situacoes = PropertyEntity({ getType: typesKnockout.selectMultiple, text: "Situação:", options: _situacoesPedagio, val: ko.observable([]), def: [], issue: 0, visible: ko.observable(true) });
    this.ValorInicial = PropertyEntity({ getType: typesKnockout.decimal, text: "Valor Inicial:" });
    this.ValorFinal = PropertyEntity({ getType: typesKnockout.decimal, text: "Valor Final:" });
    this.DataInicial = PropertyEntity({ getType: typesKnockout.date, text: "Data Inicial:", val: ko.observable(Global.DataAtual()) });
    this.DataFinal = PropertyEntity({ getType: typesKnockout.date, text: "Data Final:", val: ko.observable(Global.DataAtual()) });
    
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridPedagio.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaPedagio.Visible.visibleFade() == true) {
                _pesquisaPedagio.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaPedagio.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******

function LoadPedagio() {
    _pesquisaPedagio = new PesquisaPedagio();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridPedagio = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/Pedagio/Pesquisa", _pesquisaPedagio);

    _gridPedagio.SetPermitirEdicaoColunas(true);
    _gridPedagio.SetQuantidadeLinhasPorPagina(10);

    _relatorioPedagio = new RelatorioGlobal("Relatorios/Pedagio/BuscarDadosRelatorio", _gridPedagio, function () {
        _relatorioPedagio.loadRelatorio(function () {
            KoBindings(_pesquisaPedagio, "knockoutPesquisaPedagio", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaPedagio", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaPedagio", false);

            BuscarVeiculos(_pesquisaPedagio.Veiculo);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaPedagio);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioPedagio.gerarRelatorio("Relatorios/Pedagio/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioPedagio.gerarRelatorio("Relatorios/Pedagio/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
