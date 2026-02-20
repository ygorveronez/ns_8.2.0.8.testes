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
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Localidade.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Carga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioMonitoramentoTempoVeiculo, _gridMonitoramentoTempoVeiculo, _pesquisaMonitoramentoTempoVeiculo, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaMonitoramentoTempoVeiculo = function () {
    this.DataInicioEntregaInicial = PropertyEntity({ text: "Data Início Entrega De:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.DataInicioEntregaFinal = PropertyEntity({ text: "Até:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });

    this.DataEntregaInicial = PropertyEntity({ text: "Data Entrega De:", getType: typesKnockout.date });
    this.DataEntregaFinal = PropertyEntity({ text: "Até:", getType: typesKnockout.date });

    this.DataEmissaoInicial = PropertyEntity({ text: "Data Emissão De:", getType: typesKnockout.date });
    this.DataEmissaoFinal = PropertyEntity({ text: "Até:", getType: typesKnockout.date });

    this.DataEmissaoInicial.dateRangeLimit = this.DataEmissaoFinal;
    this.DataEmissaoFinal.dateRangeInit = this.DataEmissaoInicial;
    this.DataInicioEntregaInicial.dateRangeLimit = this.DataInicioEntregaFinal;
    this.DataInicioEntregaFinal.dateRangeInit = this.DataInicioEntregaInicial;
    this.DataEntregaInicial.dateRangeLimit = this.DataEntregaFinal;
    this.DataEntregaFinal.dateRangeInit = this.DataEntregaInicial;

    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Transportador:", issue: 69, idBtnSearch: guid() });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Origem:", issue: 16, idBtnSearch: guid() });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destino:", issue: 16, idBtnSearch: guid() });
    this.ClienteEntrega = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Cliente da Entrega:", issue: 52, idBtnSearch: guid() });
    this.Carga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), multiplesEntitiesConfig: { propDescricao: "CodigoCargaEmbarcador" } });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridMonitoramentoTempoVeiculo.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******


function loadRelatorioMonitoramentoTempoVeiculo() {

    _pesquisaMonitoramentoTempoVeiculo = new PesquisaMonitoramentoTempoVeiculo();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridMonitoramentoTempoVeiculo = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/MonitoramentoTempoVeiculo/Pesquisa", _pesquisaMonitoramentoTempoVeiculo, null, null, 10);
    _gridMonitoramentoTempoVeiculo.SetPermitirEdicaoColunas(true);

    _relatorioMonitoramentoTempoVeiculo = new RelatorioGlobal("Relatorios/MonitoramentoTempoVeiculo/BuscarDadosRelatorio", _gridMonitoramentoTempoVeiculo, function () {
        _relatorioMonitoramentoTempoVeiculo.loadRelatorio(function () {
            KoBindings(_pesquisaMonitoramentoTempoVeiculo, "knockoutPesquisaMonitoramentoTempoVeiculo");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaMonitoramentoTempoVeiculo");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaMonitoramentoTempoVeiculo");

            new BuscarFilial(_pesquisaMonitoramentoTempoVeiculo.Filial);
            new BuscarTransportadores(_pesquisaMonitoramentoTempoVeiculo.Transportador, null, null, true);
            new BuscarLocalidades(_pesquisaMonitoramentoTempoVeiculo.Origem);
            new BuscarLocalidades(_pesquisaMonitoramentoTempoVeiculo.Destino);
            new BuscarClientes(_pesquisaMonitoramentoTempoVeiculo.ClienteEntrega);
            new BuscarCargas(_pesquisaMonitoramentoTempoVeiculo.Carga);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaMonitoramentoTempoVeiculo);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioMonitoramentoTempoVeiculo.gerarRelatorio("Relatorios/MonitoramentoTempoVeiculo/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioMonitoramentoTempoVeiculo.gerarRelatorio("Relatorios/MonitoramentoTempoVeiculo/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}