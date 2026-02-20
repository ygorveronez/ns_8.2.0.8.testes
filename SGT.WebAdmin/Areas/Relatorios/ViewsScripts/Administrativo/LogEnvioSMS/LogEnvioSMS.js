/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioLogEnvioSMS, _gridLogEnvioSMS, _pesquisaLogEnvioSMS, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaLogEnvioSMS = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });

    this.NumeroNotaInicial = PropertyEntity({ text: "Número Nota Inicial: ", getType: typesKnockout.int });
    this.NumeroNotaFinal = PropertyEntity({ text: "Número Nota Final: ", getType: typesKnockout.int });

    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa:", idBtnSearch: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridLogEnvioSMS.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******


function loadRelatorioLogEnvioSMS() {

    _pesquisaLogEnvioSMS = new PesquisaLogEnvioSMS();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridLogEnvioSMS = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/LogEnvioSMS/Pesquisa", _pesquisaLogEnvioSMS, null, null, 10);
    _gridLogEnvioSMS.SetPermitirEdicaoColunas(true);

    _relatorioLogEnvioSMS = new RelatorioGlobal("Relatorios/LogEnvioSMS/BuscarDadosRelatorio", _gridLogEnvioSMS, function () {
        _relatorioLogEnvioSMS.loadRelatorio(function () {
            KoBindings(_pesquisaLogEnvioSMS, "knockoutPesquisaLogEnvioSMS");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaLogEnvioSMS");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaLogEnvioSMS");

            new BuscarClientes(_pesquisaLogEnvioSMS.Pessoa);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaLogEnvioSMS);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioLogEnvioSMS.gerarRelatorio("Relatorios/LogEnvioSMS/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioLogEnvioSMS.gerarRelatorio("Relatorios/LogEnvioSMS/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}