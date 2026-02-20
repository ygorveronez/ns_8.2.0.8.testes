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

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioLogEnvioEmail, _gridLogEnvioEmail, _pesquisaLogEnvioEmail, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaLogEnvioEmail = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridLogEnvioEmail.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******


function loadRelatorioLogEnvioEmail() {

    _pesquisaLogEnvioEmail = new PesquisaLogEnvioEmail();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridLogEnvioEmail = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/LogEnvioEmail/Pesquisa", _pesquisaLogEnvioEmail, null, null, 10);
    _gridLogEnvioEmail.SetPermitirEdicaoColunas(true);

    _relatorioLogEnvioEmail = new RelatorioGlobal("Relatorios/LogEnvioEmail/BuscarDadosRelatorio", _gridLogEnvioEmail, function () {
        _relatorioLogEnvioEmail.loadRelatorio(function () {
            KoBindings(_pesquisaLogEnvioEmail, "knockoutPesquisaLogEnvioEmail");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaLogEnvioEmail");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaLogEnvioEmail");

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaLogEnvioEmail);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioLogEnvioEmail.gerarRelatorio("Relatorios/LogEnvioEmail/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioLogEnvioEmail.gerarRelatorio("Relatorios/LogEnvioEmail/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}