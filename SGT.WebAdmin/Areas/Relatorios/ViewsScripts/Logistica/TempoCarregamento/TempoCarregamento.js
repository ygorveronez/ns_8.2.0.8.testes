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
/// <reference path="../../../../../ViewsScripts/Consultas/Localidade.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Carga.js" />
/// <reference path="../../../../../js/app.config.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Trasportador.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CentroCarregamento.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTempoCarregamentos, _pesquisaTempoCarregamentos, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioTempoCarregamentos;

var PesquisaTempoCarregamentos = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    var dataAtual = moment().format("DD/MM/YYYY");
    this.DataInicioCarregamento = PropertyEntity({ text: "Data Início: ",issue: 2, val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFimCarregamento = PropertyEntity({ text: "Data Início: ", issue: 2, val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ text: "Transportador:",issue: 69, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:",issue: 145, idBtnSearch: guid(),  visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", issue: 143, idBtnSearch: guid(), issue: 143, visible: ko.observable(true) });
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Carregamento:", issue: 320, idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:",issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridTempoCarregamentos.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******

function loadRelatorioTempoCarregamentos() {
    _pesquisaTempoCarregamentos = new PesquisaTempoCarregamentos();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridTempoCarregamentos = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/TempoCarregamento/Pesquisa", _pesquisaTempoCarregamentos, null);

    _gridTempoCarregamentos.SetPermitirEdicaoColunas(true);
    _gridTempoCarregamentos.SetQuantidadeLinhasPorPagina(10);

    _relatorioTempoCarregamentos = new RelatorioGlobal("Relatorios/TempoCarregamento/BuscarDadosRelatorio", _gridTempoCarregamentos, function () {
        _relatorioTempoCarregamentos.loadRelatorio(function () {
            KoBindings(_pesquisaTempoCarregamentos, "knockoutPesquisaTempoCarregamentos", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaTempoCarregamentos", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaTempoCarregamento", false);

            new BuscarCentrosCarregamento(_pesquisaTempoCarregamentos.CentroCarregamento);
            new BuscarMotoristas(_pesquisaTempoCarregamentos.Motorista);
            new BuscarVeiculos(_pesquisaTempoCarregamentos.Veiculo);
            new BuscarTransportadores(_pesquisaTempoCarregamentos.Transportador);
            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaTempoCarregamentos);

}

function baixarDacteClick(e) {
    var data = { CodigoTempoCarregamento: e.Codigo };
    executarDownload("Relatorios/TempoCarregamentos/DownloadDacte", data);
}

function baixarXMLTempoCarregamentoClick(e) {
    var data = { CodigoTempoCarregamento: e.Codigo };
    executarDownload("Relatorios/TempoCarregamentos/DownloadXML", data);
}


function GerarRelatorioPDFClick(e, sender) {
    _relatorioTempoCarregamentos.gerarRelatorio("Relatorios/TempoCarregamento/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioTempoCarregamentos.gerarRelatorio("Relatorios/TempoCarregamento/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}