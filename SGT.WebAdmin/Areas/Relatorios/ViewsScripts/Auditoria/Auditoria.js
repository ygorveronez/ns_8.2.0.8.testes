/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioAuditoria, _gridAuditoria, _pesquisaAuditoria, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaAuditoria = function () {
    var dataAtual = moment().format("DD/MM/YYYY");

    this.DataInicial = PropertyEntity({ text: "*Data Inicial:", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true), required: true });
    this.DataFinal = PropertyEntity({ text: "*Data Final:", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true), required: true });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Usuario = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Menu = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Menu:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.AcaoRealizada = PropertyEntity({ type: types.text, text: "Ação Realizada:", visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridAuditoria.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******


function loadRelatorioAuditoria() {
    _pesquisaAuditoria = new PesquisaAuditoria();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridAuditoria = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/AuditoriaUsuario/Pesquisa", _pesquisaAuditoria, null, null, 10);
    _gridAuditoria.SetPermitirEdicaoColunas(true);

    _relatorioAuditoria = new RelatorioGlobal("Relatorios/AuditoriaUsuario/BuscarDadosRelatorio", _gridAuditoria, function () {
        _relatorioAuditoria.loadRelatorio(function () {
            KoBindings(_pesquisaAuditoria, "knockoutPesquisaAuditoria");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaAuditoria");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaAuditoria");

            new BuscarFuncionario(_pesquisaAuditoria.Usuario, null, null, null, null, null, null, null, true);
            new BuscarHistoricoAuditoriaUsuario(_pesquisaAuditoria.Menu);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaAuditoria);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioAuditoria.gerarRelatorio("Relatorios/AuditoriaUsuario/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioAuditoria.gerarRelatorio("Relatorios/AuditoriaUsuario/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}