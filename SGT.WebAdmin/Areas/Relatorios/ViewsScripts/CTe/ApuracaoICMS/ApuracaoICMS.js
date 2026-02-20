/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioApuracaoICMS, _gridApuracaoICMS, _pesquisaApuracaoICMS, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaApuracaoICMS = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.DataEmissaoInicial = PropertyEntity({ text: "Data Emissão Inicial: ", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.DataEmissaoFinal = PropertyEntity({ text: "Data Emissão Final: ", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.DataEmissaoInicial.dateRangeLimit = this.DataEmissaoFinal;
    this.DataEmissaoFinal.dateRangeInit = this.DataEmissaoInicial;

    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid() });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid() });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid() });
    this.Recebedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Recebedor:", idBtnSearch: guid() });
    this.Expedidor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Expedidor:", idBtnSearch: guid() });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridApuracaoICMS.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function LoadApuracaoICMS() {
    _pesquisaApuracaoICMS = new PesquisaApuracaoICMS();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridApuracaoICMS = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ApuracaoICMS/Pesquisa", _pesquisaApuracaoICMS, null, null, 10);
    _gridApuracaoICMS.SetPermitirEdicaoColunas(true);

    _relatorioApuracaoICMS = new RelatorioGlobal("Relatorios/ApuracaoICMS/BuscarDadosRelatorio", _gridApuracaoICMS, function () {
        _relatorioApuracaoICMS.loadRelatorio(function () {
            KoBindings(_pesquisaApuracaoICMS, "knockoutPesquisaApuracaoICMS", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaApuracaoICMS", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaApuracaoICMS", false);

            new BuscarClientes(_pesquisaApuracaoICMS.Remetente);
            new BuscarClientes(_pesquisaApuracaoICMS.Destinatario);
            new BuscarClientes(_pesquisaApuracaoICMS.Tomador);
            new BuscarClientes(_pesquisaApuracaoICMS.Recebedor);
            new BuscarClientes(_pesquisaApuracaoICMS.Expedidor);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaApuracaoICMS);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioApuracaoICMS.gerarRelatorio("Relatorios/ApuracaoICMS/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioApuracaoICMS.gerarRelatorio("Relatorios/ApuracaoICMS/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}