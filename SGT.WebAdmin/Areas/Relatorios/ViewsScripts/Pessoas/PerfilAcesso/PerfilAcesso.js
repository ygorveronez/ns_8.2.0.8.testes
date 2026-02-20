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

var _gridPerfilAcesso, _pesquisaPerfilAcesso, _CRUDRelatorio, _CRUDFiltrosRelatorio, _relatorioPerfilAcesso;

var _statusAtivo = [
    { text: "Todos", value: "" },
    { text: "Ativo", value: true },
    { text: "Inativo", value: false }
];

var PesquisaPerfilAcesso = function () {
    this.Ativo = PropertyEntity({ val: ko.observable(""), options: _statusAtivo, text: "Situação: " });
    this.PerfilAcesso = PropertyEntity({ text: "Perfil de Acesso:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridPerfilAcesso.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******

function loadRelatorioPerfilAcesso() {
    _pesquisaPerfilAcesso = new PesquisaPerfilAcesso();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridPerfilAcesso = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/PerfilAcesso/Pesquisa", _pesquisaPerfilAcesso, null, null, 10, null, null, null, null, 100000);
    _gridPerfilAcesso.SetPermitirEdicaoColunas(true);

    _relatorioPerfilAcesso = new RelatorioGlobal("Relatorios/PerfilAcesso/BuscarDadosRelatorio", _gridPerfilAcesso, function () {
        _relatorioPerfilAcesso.loadRelatorio(function () {
            KoBindings(_pesquisaPerfilAcesso, "knockoutPesquisaPerfilAcesso", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaPerfilAcesso", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaPerfilAcesso", false);

            new BuscarPerfilAcesso(_pesquisaPerfilAcesso.PerfilAcesso, null, 0);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaPerfilAcesso);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioPerfilAcesso.gerarRelatorio("Relatorios/PerfilAcesso/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioPerfilAcesso.gerarRelatorio("Relatorios/PerfilAcesso/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}