

//********MAPEAMENTO KNOCKOUT********

var _relatorioPedidoDevolucao, _gridPedidoDevolucao, _pesquisaPedidoDevolucao, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _tipoDevolucao = [
    { text: "Todos", value: "" },
    { text: "Devolução Parcial", value: EnumTipoColetaEntregaDevolucao.Parcial },
    { text: "Devolução Total", value: EnumTipoColetaEntregaDevolucao.Total }
];

var PesquisaPedidoDevolucao = function () {
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.Pedido = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pedido:", idBtnSearch: guid() });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid() });
    this.Motivo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motivo:", idBtnSearch: guid() });
    this.NumeroNF = PropertyEntity({ getType: typesKnockout.int, text: "Número da NF:", val: ko.observable(""), def: "", configInt: { precision: 0, allowZero: false, thousands: "" }, maxlength: 12 });
    this.DataEmissaoNFInicial = PropertyEntity({ getType: typesKnockout.date, text: "Data inicial de emissão da NF:", val: ko.observable(""), def: "" });
    this.DataEmissaoNFFinal = PropertyEntity({ getType: typesKnockout.date, text: "Data final de emissão da NF:", val: ko.observable(""), def: "" });
    this.TipoDevolucao = PropertyEntity({ text: "Tipo de Devolução:", options: _tipoDevolucao, val: ko.observable(""), def: "" });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridPedidoDevolucao.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    /* BUSCA AVANÇADA */
    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaPedidoDevolucao.Visible.visibleFade() === true) {
                _pesquisaPedidoDevolucao.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaPedidoDevolucao.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel" });
}

//*********EVENTOS**********

function LoadPedidoDevolucao() {
    _pesquisaPedidoDevolucao = new PesquisaPedidoDevolucao();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridPedidoDevolucao = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/PedidoDevolucao/Pesquisa", _pesquisaPedidoDevolucao, null, null, 10);
    _gridPedidoDevolucao.SetPermitirEdicaoColunas(true);

    _relatorioPedidoDevolucao = new RelatorioGlobal("Relatorios/PedidoDevolucao/BuscarDadosRelatorio", _gridPedidoDevolucao, function () {
        _relatorioPedidoDevolucao.loadRelatorio(function () {
            KoBindings(_pesquisaPedidoDevolucao, "knockoutPesquisaPedidoDevolucao", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaPedidoDevolucao", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaPedidoDevolucao", false);

            new BuscarPedidos(_pesquisaPedidoDevolucao.Pedido);
            new BuscarCargas(_pesquisaPedidoDevolucao.Carga);
            new BuscarTransportadores(_pesquisaPedidoDevolucao.Transportador);
            new BuscarClientes(_pesquisaPedidoDevolucao.Cliente);
            new BuscarTipoOcorrencia(_pesquisaPedidoDevolucao.Motivo, null, null, null, null, null, null, null, true);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaPedidoDevolucao);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioPedidoDevolucao.gerarRelatorio("Relatorios/PedidoDevolucao/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioPedidoDevolucao.gerarRelatorio("Relatorios/PedidoDevolucao/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}