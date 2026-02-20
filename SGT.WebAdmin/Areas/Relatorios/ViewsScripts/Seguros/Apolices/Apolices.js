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
/// <reference path="../../Relatorios/Global/Relatorio.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridApolices, _pesquisaApolices, _CRUDRelatorio, _CRUDFiltrosRelatorio, _relatorioApolices;

var _tipoResponsavelSeguro = [
    { value: "", text: "Todos" },
    { value: EnumResponsavelSeguro.Transportador, text: "Transportador" },
    { value: EnumResponsavelSeguro.Embarcador, text: "Embarcador" }
];

var _seguradoraAverbacao = [
    { value: "", text: "Todos" },
    { value: EnumSeguradoraAverbacao.NaoDefinido, text: "Não Definido" },
    { value: EnumSeguradoraAverbacao.ATM, text: "ATM" },
    { value: EnumSeguradoraAverbacao.Bradesco, text: "Bradesco" },
];

var PesquisaApolices = function () {
    this.Seguradora = PropertyEntity({ text: "Seguradora:",issue: 262, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Responsavel = PropertyEntity({ text: "Responsável:", options: _tipoResponsavelSeguro, val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Averbadora = PropertyEntity({ text: "Averbadora:", options: _seguradoraAverbacao, val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.EmVigencia = PropertyEntity({ text: "Em Vigência", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:",issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridApolices.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******

function loadRelatorioApolices() {
    _pesquisaApolices = new PesquisaApolices();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridApolices = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/Apolices/Pesquisa", _pesquisaApolices);

    _gridApolices.SetPermitirEdicaoColunas(true);
    _gridApolices.SetQuantidadeLinhasPorPagina(25);

    _relatorioApolices = new RelatorioGlobal("Relatorios/Apolices/BuscarDadosRelatorio", _gridApolices, function () {
        _relatorioApolices.loadRelatorio(function () {
            KoBindings(_pesquisaApolices, "knockoutPesquisaApolices", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaApolices", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaApolices", false);

            new BuscarSeguradoras(_pesquisaApolices.Seguradora);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaApolices);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioApolices.gerarRelatorio("Relatorios/Apolices/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioApolices.gerarRelatorio("Relatorios/Apolices/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}