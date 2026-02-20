/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />

//********MAPEAMENTO KNOCKOUT********

var _relatorioPlanejamentoFrotaDia, _gridPlanejamentoFrotaDia, _pesquisaPlanejamentoFrotaDia, _CRUDRelatorio, _CRUDFiltrosRelatorio;


var PesquisaPneuPlanejamentoFrotaDia = function () {
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filial: ", idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Transportador: ", idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Veículo: ", idBtnSearch: guid() });

    this.Placa = PropertyEntity({ text: "Placa: ", val: ko.observable("") });

    this.PeriodoInicio = PropertyEntity({ text: "Período Inicial: ", getType: typesKnockout.date, visible: true });
    this.PeriodoFim = PropertyEntity({ text: "Período Final: ", getType: typesKnockout.date, visible: true });

    this.Roteirizado = PropertyEntity({ text: "Roteirizado: ", options: Global.ObterOpcoesPesquisaBooleano("Roteirizado", "Não Roteirizado"), val: ko.observable(Global.ObterOpcoesPesquisaBooleano()), def: "" });
    this.Situacao = PropertyEntity({ text: "Status: ", options: Global.ObterOpcoesPesquisaBooleano("Disponível", "Indisponível"), val: ko.observable(Global.ObterOpcoesPesquisaBooleano()), def: "" });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridPlanejamentoFrotaDia.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel" });
}

//*********EVENTOS**********

function LoadPlanejamentoFrotaDia() {
    _pesquisaPlanejamentoFrotaDia = new PesquisaPneuPlanejamentoFrotaDia();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridPlanejamentoFrotaDia = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/PlanejamentoFrotaDia/Pesquisa", _pesquisaPlanejamentoFrotaDia, null, null, 10);
    _gridPlanejamentoFrotaDia.SetPermitirEdicaoColunas(true);

    _relatorioPlanejamentoFrotaDia = new RelatorioGlobal("Relatorios/PlanejamentoFrotaDia/BuscarDadosRelatorio", _gridPlanejamentoFrotaDia, function () {
        _relatorioPlanejamentoFrotaDia.loadRelatorio(function () {
            KoBindings(_pesquisaPlanejamentoFrotaDia, "knockoutPesquisaPlanejamentoFrotaDia", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaPlanejamentoFrotaDia", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaPlanejamentoFrotaDia", false);

            new BuscarVeiculos(_pesquisaPlanejamentoFrotaDia.Veiculo);
            new BuscarFilial(_pesquisaPlanejamentoFrotaDia.Filial);
            new BuscarTransportadores(_pesquisaPlanejamentoFrotaDia.Transportador);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaPlanejamentoFrotaDia);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioPlanejamentoFrotaDia.gerarRelatorio("Relatorios/PlanejamentoFrotaDia/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioPlanejamentoFrotaDia.gerarRelatorio("Relatorios/PlanejamentoFrotaDia/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}