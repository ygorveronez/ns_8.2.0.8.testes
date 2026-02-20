/// <reference path="../../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../../js/Global/CRUD.js" />
/// <reference path="../../../../../js/Global/knockout-3.1.0.js" />
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

//*******MAPEAMENTO KNOUCKOUT*******

var _gridCheckListUsuario, _pesquisaCheckListUsuario, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioCheckListUsuario;

var PesquisaCheckListUsuario = function () {
    this.DataPreenchimentoInicial = PropertyEntity({ text: "Data Preenchimento Inicial: ", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataPreenchimentoFinal = PropertyEntity({ text: "Data Preenchimento Final: ", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid() });
    this.TipoGROT = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo GROT:", idBtnSearch: guid() });


    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridCheckListUsuario.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******

function loadRelatorioCheckListUsuario() {
    _pesquisaCheckListUsuario = new PesquisaCheckListUsuario();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridCheckListUsuario = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/CheckListUsuario/Pesquisa", _pesquisaCheckListUsuario, null);

    _gridCheckListUsuario.SetPermitirEdicaoColunas(true);
    _gridCheckListUsuario.SetQuantidadeLinhasPorPagina(10);

    _relatorioCheckListUsuario = new RelatorioGlobal("Relatorios/CheckListUsuario/BuscarDadosRelatorio", _gridCheckListUsuario, function () {
        _relatorioCheckListUsuario.loadRelatorio(function () {
            KoBindings(_pesquisaCheckListUsuario, "knockoutPesquisaCheckListUsuario", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaCheckListUsuario", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaCheckListUsuario", false);

            new BuscarOperador(_pesquisaCheckListUsuario.Usuario);
            new BuscarTipoGrot(_pesquisaCheckListUsuario.TipoGROT);
            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaCheckListUsuario);

}
function GerarRelatorioPDFClick(e, sender) {
    _relatorioCheckListUsuario.gerarRelatorio("Relatorios/CheckListUsuario/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioCheckListUsuario.gerarRelatorio("Relatorios/CheckListUsuario/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}