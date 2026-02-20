/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDRelatorio;
var _CRUDFiltrosRelatorio;
var _gridMovimentacaoPneuVeiculo;
var _pesquisaMovimentacaoPneuVeiculo;
var _relatorioMovimentacaoPneuVeiculo;

/*
 * Declaração das Classes
 */

var PesquisaMovimentacaoPneuVeiculo = function () {
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Veiculo:"), idBtnSearch: guid(), required: true });
    this.OcultarDados = PropertyEntity({ text: "Ocultar Dados dos Pneus do Veículo?", getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({ eventClick: carregarMovimentacaoPneuVeiculo, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true) });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
}

/*
 * Declaração das Funções de Inicialização
 */

function LoadMovimentacaoPneuVeiculo() {
    _pesquisaMovimentacaoPneuVeiculo = new PesquisaMovimentacaoPneuVeiculo();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();
    _gridMovimentacaoPneuVeiculo = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/MovimentacaoPneuVeiculo/Pesquisa", _pesquisaMovimentacaoPneuVeiculo);

    _gridMovimentacaoPneuVeiculo.SetPermitirEdicaoColunas(true);
    _gridMovimentacaoPneuVeiculo.SetQuantidadeLinhasPorPagina(20);

    _relatorioMovimentacaoPneuVeiculo = new RelatorioGlobal("Relatorios/MovimentacaoPneuVeiculo/BuscarDadosRelatorio", _gridMovimentacaoPneuVeiculo, function () {
        _relatorioMovimentacaoPneuVeiculo.loadRelatorio(function () {
            KoBindings(_pesquisaMovimentacaoPneuVeiculo, "knockoutPesquisaMovimentacaoPneuVeiculo", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaMovimentacaoPneuVeiculo", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaMovimentacaoPneuVeiculo", false);

            new BuscarVeiculos(_pesquisaMovimentacaoPneuVeiculo.Veiculo);

            var knoutRelatorio = _relatorioMovimentacaoPneuVeiculo.obterKnoutRelatorio();

            knoutRelatorio.AgruparRelatorio.visible(false);
            knoutRelatorio.ExibirSumarios.visible(false);
            knoutRelatorio.OcultarDetalhe.visible(false);
            knoutRelatorio.NovaPaginaAposAgrupamento.visible(false);
            knoutRelatorio.OrientacaoRelatorio.visible(false);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaMovimentacaoPneuVeiculo);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function GerarRelatorioPDFClick() {
    _relatorioMovimentacaoPneuVeiculo.gerarRelatorio("Relatorios/MovimentacaoPneuVeiculo/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function carregarMovimentacaoPneuVeiculo() {
    if (ValidarCamposObrigatorios(_pesquisaMovimentacaoPneuVeiculo))
        _gridMovimentacaoPneuVeiculo.CarregarGrid();
    else
        exibirMensagem("Atencao", "Campo Obrigatório", "Por Favor, informe os campos obrigatórios");
}