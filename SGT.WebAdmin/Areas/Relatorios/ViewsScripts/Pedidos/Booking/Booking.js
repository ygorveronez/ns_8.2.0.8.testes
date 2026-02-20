

//********MAPEAMENTO KNOCKOUT********

var _relatorioBooking, _gridBooking, _pesquisaBooking, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _tipoDevolucao = [
    { text: "Todos", value: "" },
    { text: "Devolução Parcial", value: EnumTipoColetaEntregaDevolucao.Parcial },
    { text: "Devolução Total", value: EnumTipoColetaEntregaDevolucao.Total }
];

var PesquisaBooking = function () {
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Fim: ", dateRangeInit: this.DataInicial, getType: typesKnockout.date });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicio;

    this.NumeroEXP = PropertyEntity({ text: "Número EXP: ", getType: typesKnockout.string });
    this.NumeroBooking = PropertyEntity({ text: "Número Booking: ", getType: typesKnockout.string });
    this.NumeroCarga = PropertyEntity({ text: "Número da Carga: ", getType: typesKnockout.string });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoCancelamentoOcorrencia.Todas), options: EnumStatusControleMaritimo.obterOpcoesPesquisa(), def: EnumStatusControleMaritimo.Todas, text: "Situação: " });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Origem:", idBtnSearch: guid() });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destino:", idBtnSearch: guid() });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridBooking.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    /* BUSCA AVANÇADA */
    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaBooking.Visible.visibleFade() === true) {
                _pesquisaBooking.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaBooking.Visible.visibleFade(true);
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

function LoadBooking() {
    _pesquisaBooking = new PesquisaBooking();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridBooking = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/Booking/Pesquisa", _pesquisaBooking, null, null, 10);
    _gridBooking.SetPermitirEdicaoColunas(true);

    _relatorioBooking = new RelatorioGlobal("Relatorios/Booking/BuscarDadosRelatorio", _gridBooking, function () {
        _relatorioBooking.loadRelatorio(function () {
            KoBindings(_pesquisaBooking, "knockoutPesquisaBooking", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaBooking", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaBooking", false);

            new BuscarFilial(_pesquisaBooking.Filial);
            new BuscarLocalidades(_pesquisaBooking.Origem);
            new BuscarLocalidades(_pesquisaBooking.Destino);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaBooking);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioBooking.gerarRelatorio("Relatorios/Booking/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioBooking.gerarRelatorio("Relatorios/Booking/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}