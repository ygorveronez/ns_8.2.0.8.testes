/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoDevolucaoPallet.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioDevolucaoPallets, _gridDevolucaoPallets, _pesquisaDevolucaoPallets, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaDevolucaoPallets = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS ? "Empresa/Filial" : "Transportador:"), idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });

    this.NumeroCarga = PropertyEntity({ text: "Carga:", maxlength: 50 });
    this.NumeroNotaFiscal = PropertyEntity({ text: "Nota Fiscal:", getType: typesKnockout.int, maxlength: 15 });
    this.Situacao = PropertyEntity({ text: "Situação:", options: EnumSituacaoDevolucaoPallet.obterOpcoesPesquisa(), val: ko.observable(EnumSituacaoDevolucaoPallet.Todas), def: EnumSituacaoDevolucaoPallet.Todas, visible: ko.observable(true) });

    this.DataInicio = PropertyEntity({ text: "Data Inicial (Devolução):", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFim = PropertyEntity({ text: "Data Final (Devolução):", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridDevolucaoPallets.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaDevolucaoPallets.Visible.visibleFade()) {
                _pesquisaDevolucaoPallets.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaDevolucaoPallets.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function LoadDevolucaoPallets() {
    _pesquisaDevolucaoPallets = new PesquisaDevolucaoPallets();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridDevolucaoPallets = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/DevolucaoPallets/Pesquisa", _pesquisaDevolucaoPallets);

    _gridDevolucaoPallets.SetPermitirEdicaoColunas(true);
    _gridDevolucaoPallets.SetQuantidadeLinhasPorPagina(20);

    _relatorioDevolucaoPallets = new RelatorioGlobal("Relatorios/DevolucaoPallets/BuscarDadosRelatorio", _gridDevolucaoPallets, function () {
        _relatorioDevolucaoPallets.loadRelatorio(function () {
            KoBindings(_pesquisaDevolucaoPallets, "knockoutPesquisaDevolucaoPallets", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaDevolucaoPallets", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaDevolucaoPallets", false);

            new BuscarTransportadores(_pesquisaDevolucaoPallets.Transportador);
            new BuscarMotoristas(_pesquisaDevolucaoPallets.Motorista);
            new BuscarVeiculos(_pesquisaDevolucaoPallets.Veiculo);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaDevolucaoPallets);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe)
        _pesquisaDevolucaoPallets.Transportador.visible(false);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioDevolucaoPallets.gerarRelatorio("Relatorios/DevolucaoPallets/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioDevolucaoPallets.gerarRelatorio("Relatorios/DevolucaoPallets/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
