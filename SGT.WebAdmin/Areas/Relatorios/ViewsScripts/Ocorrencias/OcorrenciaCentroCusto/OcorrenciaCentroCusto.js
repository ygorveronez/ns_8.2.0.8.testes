/// <reference path="../../../../../js/app.config.js" />
/// <reference path="../../../../../js/Global/CRUD.js" />
/// <reference path="../../../../../js/Global/Grid.js" />
/// <reference path="../../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../../js/Global/Rest.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Carga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoOcorrencia.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridOcorrenciaCentroCusto;
var _pesquisaOcorrenciaCentroCusto;
var _CRUDRelatorio;
var _CRUDFiltrosRelatorio;
var _relatorioOcorrenciaCentroCusto;

var _configuracaoNumeroInteiro = {
    precision: 0,
    allowZero: false,
    allowNegative: false,
    thousands: ""
};

/*
 * Declaração das Classes
 */

var PesquisaOcorrenciaCentroCusto = function () {
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid() });
    this.DataOcorrenciaInicial = PropertyEntity({ text: "Data Ocorrência Inicial: ", getType: typesKnockout.date });
    this.DataOcorrenciaLimite = PropertyEntity({ text: "Data Ocorrência Final: ", getType: typesKnockout.date });
    this.NumeroOcorrencia = PropertyEntity({ text: "Número da Ocorrência:", getType: typesKnockout.int, configInt: _configuracaoNumeroInteiro });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });

    this.DataOcorrenciaInicial.dateRangeLimit = this.DataOcorrenciaLimite;
    this.DataOcorrenciaLimite.dateRangeInit = this.DataOcorrenciaInicial;

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridOcorrenciaCentroCusto.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

/*
 * Declaração das Funções de Inicialização
 */

function LoadOcorrenciaCentroCusto() {
    _pesquisaOcorrenciaCentroCusto = new PesquisaOcorrenciaCentroCusto();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridOcorrenciaCentroCusto = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/OcorrenciaCentroCusto/Pesquisa", _pesquisaOcorrenciaCentroCusto);

    _gridOcorrenciaCentroCusto.SetPermitirEdicaoColunas(true);
    _gridOcorrenciaCentroCusto.SetQuantidadeLinhasPorPagina(10);

    _relatorioOcorrenciaCentroCusto = new RelatorioGlobal("Relatorios/OcorrenciaCentroCusto/BuscarDadosRelatorio", _gridOcorrenciaCentroCusto, function () {
        _relatorioOcorrenciaCentroCusto.loadRelatorio(function () {
            KoBindings(_pesquisaOcorrenciaCentroCusto, "knockoutPesquisaOcorrenciaCentroCusto", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaOcorrenciaCentroCusto", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaOcorrenciaCentroCusto", false);

            new BuscarCargas(_pesquisaOcorrenciaCentroCusto.Carga)
            new BuscarTransportadores(_pesquisaOcorrenciaCentroCusto.Transportador);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaOcorrenciaCentroCusto);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function GerarRelatorioPDFClick() {
    _relatorioOcorrenciaCentroCusto.gerarRelatorio("Relatorios/OcorrenciaCentroCusto/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick() {
    _relatorioOcorrenciaCentroCusto.gerarRelatorio("Relatorios/OcorrenciaCentroCusto/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
