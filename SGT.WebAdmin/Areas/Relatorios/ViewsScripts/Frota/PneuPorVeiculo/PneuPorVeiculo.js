/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoBandaRodagemPneu.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoPneuTMS.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Pneu.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ModeloPneu.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/MarcaPneu.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/SegmentoVeiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CentroResultado.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />

//********MAPEAMENTO KNOCKOUT********

let _relatorioPneuPorVeiculo, _gridPneuPorVeiculo, _pesquisaPneuPorVeiculo, _CRUDRelatorio, _CRUDFiltrosRelatorio;


let PesquisaPneuPorVeiculo = function () {
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo: ", idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista: ", idBtnSearch: guid() });
    this.MostrarSomentePosicoesVazias = PropertyEntity({ text: "Mostrar Somente Posições Vazias?", getType: typesKnockout.bool });
    this.ModeloVeicular = PropertyEntity({ codEntity: ko.observable(0), text: "Modelo Veicular:", idBtnSearch: guid(), issue: 0 });
    this.ModeloPneu = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo do pneu: ", idBtnSearch: guid() });
    this.MarcaPneu = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Marca do pneu: ", idBtnSearch: guid() });
    this.Reboque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Reboque:", idBtnSearch: guid() });
    this.Segmento = PropertyEntity({ type: types.entity, type: types.multiplesEntities, codEntity: ko.observable(0), text: "Segmento: ", idBtnSearch: guid() });
    this.CentroResultado = PropertyEntity({ type: types.entity, type: types.multiplesEntities, codEntity: ko.observable(0), text: "Centro Resultado: ", idBtnSearch: guid() });
};

let CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridPneuPorVeiculo.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

let CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel" });
}

//*********EVENTOS**********

function LoadPneuPorVeiculo() {
    _pesquisaPneuPorVeiculo = new PesquisaPneuPorVeiculo();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridPneuPorVeiculo = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/PneuPorVeiculo/Pesquisa", _pesquisaPneuPorVeiculo, null, null, 10);
    _gridPneuPorVeiculo.SetPermitirEdicaoColunas(true);

    _relatorioPneuPorVeiculo = new RelatorioGlobal("Relatorios/PneuPorVeiculo/BuscarDadosRelatorio", _gridPneuPorVeiculo, function () {
        _relatorioPneuPorVeiculo.loadRelatorio(function () {
            KoBindings(_pesquisaPneuPorVeiculo, "knockoutPesquisaPneuPorVeiculo", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaPneuPorVeiculo", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaPneuPorVeiculo", false);

            BuscarVeiculos(_pesquisaPneuPorVeiculo.Veiculo);
            BuscarModeloPneu(_pesquisaPneuPorVeiculo.ModeloPneu);
            BuscarMarcaPneu(_pesquisaPneuPorVeiculo.MarcaPneu);
            BuscarMotoristasPorStatus(_pesquisaPneuPorVeiculo.Motorista);
            BuscarModelosVeicularesCarga(_pesquisaPneuPorVeiculo.ModeloVeicular);
            BuscarSegmentoVeiculo(_pesquisaPneuPorVeiculo.Segmento);
            BuscarCentroResultado(_pesquisaPneuPorVeiculo.CentroResultado);
            BuscarReboques(_pesquisaPneuPorVeiculo.Reboque);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaPneuPorVeiculo);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioPneuPorVeiculo.gerarRelatorio("Relatorios/PneuPorVeiculo/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioPneuPorVeiculo.gerarRelatorio("Relatorios/PneuPorVeiculo/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}