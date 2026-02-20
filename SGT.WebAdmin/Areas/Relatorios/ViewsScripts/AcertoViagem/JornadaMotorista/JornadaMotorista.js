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
/// <reference path="../../../../../ViewsScripts/Consultas/Localidade.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Produto.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoColaborador.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Licenca.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridJornadaMotorista, _pesquisaJornadaMotorista, _CRUDRelatorio, _relatorioJornadaMotorista, _CRUDFiltrosRelatorio;


var PesquisaJornadaMotorista = function () {
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridJornadaMotorista.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******

function loadRelatorioJornadaMotorista() {

    _pesquisaJornadaMotorista = new PesquisaJornadaMotorista();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridJornadaMotorista = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "/Relatorios/JornadaMotorista/Pesquisa", _pesquisaJornadaMotorista, null, null, 10);
    _gridJornadaMotorista.SetPermitirEdicaoColunas(true);

    _relatorioJornadaMotorista = new RelatorioGlobal("Relatorios/JornadaMotorista/BuscarDadosRelatorio", _gridJornadaMotorista, function () {
        _relatorioJornadaMotorista.loadRelatorio(function () {
            KoBindings(_pesquisaJornadaMotorista, "knockoutPesquisaJornadaMotorista");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaJornadaMotorista");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaJornadaMotorista");

            new BuscarMotoristas(_pesquisaJornadaMotorista.Motorista);
            new BuscarVeiculos(_pesquisaJornadaMotorista.Veiculo);
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaJornadaMotorista);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioJornadaMotorista.gerarRelatorio("Relatorios/JornadaMotorista/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioJornadaMotorista.gerarRelatorio("Relatorios/JornadaMotorista/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
