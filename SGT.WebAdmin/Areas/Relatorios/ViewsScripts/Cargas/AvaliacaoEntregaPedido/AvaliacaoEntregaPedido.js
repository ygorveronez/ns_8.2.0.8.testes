/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridAvaliacaoEntregaPedido, _pesquisaAvaliacaoEntregaPedido, _CRUDRelatorio, _relatorioAvaliacaoEntregaPedido, _CRUDFiltrosRelatorio;

var PesquisaAvaliacaoEntregaPedido = function () {
    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });

    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;
    this.NumeroCarga = PropertyEntity({ text: "Número Carga: " });

    this.Transportadores = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Transportador: ", idBtnSearch: guid() });
    this.Veiculos = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Veículo: ", idBtnSearch: guid() });
    this.Destinatarios = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Destinatário: ", idBtnSearch: guid() });
    this.Motivos = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Motivos: ", idBtnSearch: guid() });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridAvaliacaoEntregaPedido.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaAvaliacaoEntregaPedido.Visible.visibleFade()) {
                _pesquisaAvaliacaoEntregaPedido.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaAvaliacaoEntregaPedido.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(false)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function LoadRelatorioAvaliacaoEntregaPedido() {
    _pesquisaAvaliacaoEntregaPedido = new PesquisaAvaliacaoEntregaPedido();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridAvaliacaoEntregaPedido = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "/Relatorios/AvaliacaoEntregaPedido/Pesquisa", _pesquisaAvaliacaoEntregaPedido, null, null, 10, null, null, null, null, 20);
    _gridAvaliacaoEntregaPedido.SetPermitirEdicaoColunas(true);

    _relatorioAvaliacaoEntregaPedido = new RelatorioGlobal("Relatorios/AvaliacaoEntregaPedido/BuscarDadosRelatorio", _gridAvaliacaoEntregaPedido, function () {
        _relatorioAvaliacaoEntregaPedido.loadRelatorio(function () {
            KoBindings(_pesquisaAvaliacaoEntregaPedido, "knockoutPesquisaAvaliacaoEntregaPedido");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaAvaliacaoEntregaPedido");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaAvaliacaoEntregaPedido");

            new BuscarTransportadores(_pesquisaAvaliacaoEntregaPedido.Transportadores, null, null, true);
            new BuscarVeiculos(_pesquisaAvaliacaoEntregaPedido.Veiculos);
            new BuscarClientes(_pesquisaAvaliacaoEntregaPedido.Destinatarios);
            new BuscarMotivoAvaliacao(_pesquisaAvaliacaoEntregaPedido.Motivos);

        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaAvaliacaoEntregaPedido);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioAvaliacaoEntregaPedido.gerarRelatorio("Relatorios/AvaliacaoEntregaPedido/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioAvaliacaoEntregaPedido.gerarRelatorio("Relatorios/AvaliacaoEntregaPedido/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}