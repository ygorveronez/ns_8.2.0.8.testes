/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ModeloDocumentoFiscal.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumStatusAverbacaoMDFe.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridMDFesAverbados, _pesquisaMDFesAverbados, _CRUDRelatorio, _CRUDFiltrosRelatorio, _relatorioMDFesAverbados;

var PesquisaMDFesAverbados = function () {
    const filtroPorTransp = _CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiCTe;

    this.DataInicialEmissao = PropertyEntity({ text: "Data Emissão Inicial: ", issue: 2, val: ko.observable(""), def: "", getType: typesKnockout.date });
    this.DataFinalEmissao = PropertyEntity({ text: "Data Emissão Final: ", issue: 2, val: ko.observable(""), def: "", getType: typesKnockout.date });
    this.DataInicialEmissao.dateRangeLimit = this.DataFinalEmissao;
    this.DataFinalEmissao.dateRangeInit = this.DataInicialEmissao;

    this.Status = PropertyEntity({ text: "Situação:", options: EnumStatusAverbacaoMDFe.obterOpcoesPesquisa(), val: ko.observable(EnumStatusAverbacaoMDFe.Sucesso), def: EnumStatusAverbacaoMDFe.Sucesso, visible: ko.observable(true) });

    this.Transportador = PropertyEntity({ text: "Transportador:", issue: 69, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: filtroPorTransp });
    this.Seguradora = PropertyEntity({ text: "Seguradora:", issue: 262, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.ModeloDocumentoFiscal = PropertyEntity({ text: "Modelo Documento Fiscal:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridMDFesAverbados.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function loadRelatorioMDFesAverbados() {
    _pesquisaMDFesAverbados = new PesquisaMDFesAverbados();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridMDFesAverbados = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/MDFesAverbados/Pesquisa", _pesquisaMDFesAverbados);

    _gridMDFesAverbados.SetPermitirEdicaoColunas(true);
    _gridMDFesAverbados.SetQuantidadeLinhasPorPagina(25);

    _relatorioMDFesAverbados = new RelatorioGlobal("Relatorios/MDFesAverbados/BuscarDadosRelatorio", _gridMDFesAverbados, function () {
        _relatorioMDFesAverbados.loadRelatorio(function () {
            KoBindings(_pesquisaMDFesAverbados, "knockoutPesquisaCTes", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaCTes", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaCTe", false);

            new BuscarTransportadores(_pesquisaMDFesAverbados.Transportador);
            new BuscarSeguradoras(_pesquisaMDFesAverbados.Seguradora);
            new BuscarModeloDocumentoFiscal(_pesquisaMDFesAverbados.ModeloDocumentoFiscal);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaMDFesAverbados);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioMDFesAverbados.gerarRelatorio("Relatorios/MDFesAverbados/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioMDFesAverbados.gerarRelatorio("Relatorios/MDFesAverbados/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}