/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridPracaPedagio, _pesquisaPracaPedagio, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioPracaPedagio;

var PesquisaPracaPedagio = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.Descricao = PropertyEntity({ text: "Descrição:" });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });
    this.Rodovia = PropertyEntity({ text: "Rodovia:", maxlength: 150 });

    this.DataTarifaInicial = PropertyEntity({ getType: typesKnockout.date, text: "Data da Tarifa Inicial:" });
    this.DataTarifaFinal = PropertyEntity({ getType: typesKnockout.date, text: "Data da Tarifa Final:" });

    this.DataTarifaInicial.dateRangeLimit = this.DataTarifaFinal;
    this.DataTarifaFinal.dateRangeInit = this.DataTarifaInicial;

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid() });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridPracaPedagio.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function LoadPracaPedagio() {
    _pesquisaPracaPedagio = new PesquisaPracaPedagio();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridPracaPedagio = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/PracaPedagio/Pesquisa", _pesquisaPracaPedagio);

    _gridPracaPedagio.SetPermitirEdicaoColunas(true);
    _gridPracaPedagio.SetQuantidadeLinhasPorPagina(10);

    _relatorioPracaPedagio = new RelatorioGlobal("Relatorios/PracaPedagio/BuscarDadosRelatorio", _gridPracaPedagio, function () {
        _relatorioPracaPedagio.loadRelatorio(function () {
            KoBindings(_pesquisaPracaPedagio, "knockoutPesquisaPracaPedagio", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaPracaPedagio", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaPracaPedagio", false);


            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaPracaPedagio);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioPracaPedagio.gerarRelatorio("Relatorios/PracaPedagio/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioPracaPedagio.gerarRelatorio("Relatorios/PracaPedagio/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}