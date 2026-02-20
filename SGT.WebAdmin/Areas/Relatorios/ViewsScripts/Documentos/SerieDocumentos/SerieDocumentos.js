/// <reference path="../../../../../ViewsScripts/Consultas/Empresa.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ModeloDocumentoFiscal.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/SerieEmpresa.js" />
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
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../js/app.config.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridSerieDocumentos, _pesquisaSerieDocumentos, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioSerieDocumentos;

var PesquisaSerieDocumentos = function () {
    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), defCodEntity: "", text: "Empresa:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Serie = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), defCodEntity: "", text: "Série:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.ModeloDocumentoFiscal = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(""), defCodEntity: "", text: "Modelo Documento Fiscal:", idBtnSearch: guid(), visible: ko.observable(true) });

};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridSerieDocumentos.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function LoadSerieDocumentos() {
    _pesquisaSerieDocumentos = new PesquisaSerieDocumentos();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridSerieDocumentos = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/SerieDocumentos/Pesquisa", _pesquisaSerieDocumentos);

    _gridSerieDocumentos.SetPermitirEdicaoColunas(true);
    _gridSerieDocumentos.SetQuantidadeLinhasPorPagina(10);

    _relatorioSerieDocumentos = new RelatorioGlobal("Relatorios/SerieDocumentos/BuscarDadosRelatorio", _gridSerieDocumentos, function () {
        _relatorioSerieDocumentos.loadRelatorio(function () {
            KoBindings(_pesquisaSerieDocumentos, "knockoutPesquisaSerieDocumentos", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaSerieDocumentos", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaSerieDocumentos", false);

            new BuscarEmpresa(_pesquisaSerieDocumentos.Empresa);
            new BuscarModeloDocumentoFiscal(_pesquisaSerieDocumentos.ModeloDocumentoFiscal);
            new BuscarSerieEmpresa(_pesquisaSerieDocumentos.Serie);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaSerieDocumentos);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioSerieDocumentos.gerarRelatorio("Relatorios/SerieDocumentos/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioSerieDocumentos.gerarRelatorio("Relatorios/SerieDocumentos/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}