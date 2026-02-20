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
/// <reference path="../../../../../ViewsScripts/Consultas/TipoOperacao.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridCompetencia, _pesquisaCompetencia, _CRUDRelatorio, _relatorioCompetencia, _CRUDFiltrosRelatorio;

var PesquisaCompetencia = function () {
    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });

    this.DataEmissaoInicial = PropertyEntity({ text: "Data Emissão Inicial: ", issue: 2, getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataEmissaoFinal = PropertyEntity({ text: "Data Emissão Final: ", issue: 2, getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataEmissaoInicial.dateRangeLimit = this.DataEmissaoFinal;
    this.DataEmissaoFinal.dateRangeInit = this.DataEmissaoInicial;

    this.DataCargaInicial = PropertyEntity({ text: "Data Carga Inicial: ", issue: 2, getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataCargaFinal = PropertyEntity({ text: "Data Carga Final: ", issue: 2, getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataCargaInicial.dateRangeLimit = this.DataCargaFinal;
    this.DataCargaFinal.dateRangeInit = this.DataCargaInicial;

    this.DataEmissaoCTeInicial = PropertyEntity({ text: "Data Emissão CTe Inicial: ", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataEmissaoCTeFinal = PropertyEntity({ text: "Data Emissão CTe Final: ", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataEmissaoCTeInicial.dateRangeLimit = this.DataEmissaoCTeFinal;
    this.DataEmissaoCTeFinal.dateRangeInit = this.DataEmissaoCTeInicial;

    this.DataEmissaoNotaInicial = PropertyEntity({ text: "Data Emissão Nota Inicial: ", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataEmissaoNotaFinal = PropertyEntity({ text: "Data Emissão Nota Final: ", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataEmissaoNotaInicial.dateRangeLimit = this.DataEmissaoNotaFinal;
    this.DataEmissaoNotaFinal.dateRangeInit = this.DataEmissaoNotaInicial;

    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Número Carga:", val: ko.observable(""), def: "" });
    this.NumeroValePedagio = PropertyEntity({ text: "Número Vale Pedágio: ", val: ko.observable(""), def: "" });

    this.VisualizarTambemDocumentosAguardandoProvisao = PropertyEntity({ text: "Visualizar também documentos aguardando provisão", var: ko.observable(false), getType: typesKnockout.bool, def: false });

    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Operação:", idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid() });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridCompetencia.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaCompetencia.Visible.visibleFade()) {
                _pesquisaCompetencia.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaCompetencia.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function LoadRelatorioCompetencia() {

    _pesquisaCompetencia = new PesquisaCompetencia();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridCompetencia = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "/Relatorios/Competencia/Pesquisa", _pesquisaCompetencia, null, null, 10);
    _gridCompetencia.SetPermitirEdicaoColunas(true);

    _relatorioCompetencia = new RelatorioGlobal("Relatorios/Competencia/BuscarDadosRelatorio", _gridCompetencia, function () {
        _relatorioCompetencia.loadRelatorio(function () {
            KoBindings(_pesquisaCompetencia, "knockoutPesquisaCompetencia");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaCompetencia");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaCompetencia");

            new BuscarTiposOperacao(_pesquisaCompetencia.TipoOperacao);
            new BuscarFilial(_pesquisaCompetencia.Filial);
            new BuscarTransportadores(_pesquisaCompetencia.Transportador);
            new BuscarClientes(_pesquisaCompetencia.Tomador);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaCompetencia);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioCompetencia.gerarRelatorio("Relatorios/Competencia/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioCompetencia.gerarRelatorio("Relatorios/Competencia/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}