/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoFilialSolicitacaoAbastecimentoGas.js" />

var _gridConsolidacaoGas;
var _pesquisaConsolidacaoGas;
var _CRUDFiltrosRelatorio;
var _relatorioConsolidacaoGas;

var opcoesSimNao = [
    { text: "Todos", value: 2 },
    { text: "Sim", value: 1},
    { text: "Não", value: 0 }
]

var PesquisaConsolidacaoGas = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, val: ko.observable(Global.DataHora()), def: Global.DataAtual(), required: false });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date, val: ko.observable(""), required: false });
    this.DataFinal.dateRangeInit = this.DataInicial;
    this.DataInicial.dateRangeLimit = this.DataFinal;

    this.Base = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Base:", idBtnSearch: guid() });

    this.DisponibilidadeTransferencia = PropertyEntity({ text: "Disponibilidade de Transferência para o próximo dia:", options: opcoesSimNao, val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true) });
    this.VolumeRodoviario = PropertyEntity({ text: "Volume Rodoviário Para carregamento no próximo dia:", options: opcoesSimNao, val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true) });
    this.TipoFilial = PropertyEntity({ text: "Tipo Filial:", options: EnumTipoFilialSolicitacaoAbastecimentoGas.obterOpcoes(), val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true) });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridConsolidacaoGas.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};


function loadRelatorioConsolidacaoGas() {
    _pesquisaConsolidacaoGas = new PesquisaConsolidacaoGas();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridConsolidacaoGas = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ConsolidacaoGas/Pesquisa", _pesquisaConsolidacaoGas, null, null, 10, null, null, null, null, 50);
    _gridConsolidacaoGas.SetPermitirEdicaoColunas(true);

    _relatorioConsolidacaoGas = new RelatorioGlobal("Relatorios/ConsolidacaoGas/BuscarDadosRelatorio", _gridConsolidacaoGas, function () {
        _relatorioConsolidacaoGas.loadRelatorio(function () {
            KoBindings(_pesquisaConsolidacaoGas, "knockoutPesquisaConsolidacaoGas", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaConsolidacaoGas", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaConsolidacaoGas", false);

            new BuscarClientes(_pesquisaConsolidacaoGas.Base);
            
            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaConsolidacaoGas);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioConsolidacaoGas.gerarRelatorio("Relatorios/ConsolidacaoGas/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioConsolidacaoGas.gerarRelatorio("Relatorios/ConsolidacaoGas/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}