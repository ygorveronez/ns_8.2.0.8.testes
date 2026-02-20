/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoOrdemCompra.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridOrdemCompra, _pesquisaOrdemCompra, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioOrdemCompra;

var PesquisaOrdemCompra = function () {
    this.NumeroInicial = PropertyEntity({ text: "Número Inicial:", getType: typesKnockout.int });
    this.NumeroFinal = PropertyEntity({ text: "Número Final:", getType: typesKnockout.int });
    this.DataGeracaoInicio = PropertyEntity({ text: "Data Geração Início:", getType: typesKnockout.date });
    this.DataGeracaoFim = PropertyEntity({ text: "Data Geração Fim:", getType: typesKnockout.date });
    this.DataPrevisaoInicio = PropertyEntity({ text: "Data Previsão Início:", getType: typesKnockout.date });
    this.DataPrevisaoFim = PropertyEntity({ text: "Data Previsão Fim:", getType: typesKnockout.date });
    this.Fornecedor = PropertyEntity({ text: "Fornecedor:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ text: "Transportador:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid() });
    this.Situacao = PropertyEntity({ text: "Situação:", val: ko.observable(EnumSituacaoOrdemCompra.Todas), options: EnumSituacaoOrdemCompra.obterOpcoesPesquisa(), def: EnumSituacaoOrdemCompra.Todas });

    this.Produto = PropertyEntity({ text: "Produto:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid() });
    this.Operador = PropertyEntity({ text: "Operador:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ text: "Veículo:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid() });

    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridOrdemCompra.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaOrdemCompra.Visible.visibleFade()) {
                _pesquisaOrdemCompra.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaOrdemCompra.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function loadRelatorioOrdemCompra() {
    _pesquisaOrdemCompra = new PesquisaOrdemCompra();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridOrdemCompra = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/OrdemCompra/Pesquisa", _pesquisaOrdemCompra);

    _gridOrdemCompra.SetPermitirEdicaoColunas(true);
    _gridOrdemCompra.SetQuantidadeLinhasPorPagina(10);

    _relatorioOrdemCompra = new RelatorioGlobal("Relatorios/OrdemCompra/BuscarDadosRelatorio", _gridOrdemCompra, function () {
        _relatorioOrdemCompra.loadRelatorio(function () {
            KoBindings(_pesquisaOrdemCompra, "knockoutPesquisaOrdemCompra", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaOrdemCompra", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaOrdemCompra", false);

            new BuscarClientes(_pesquisaOrdemCompra.Fornecedor);
            new BuscarClientes(_pesquisaOrdemCompra.Transportador);
            new BuscarProdutoTMS(_pesquisaOrdemCompra.Produto);
            new BuscarFuncionario(_pesquisaOrdemCompra.Operador);
            new BuscarVeiculos(_pesquisaOrdemCompra.Veiculo);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaOrdemCompra);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioOrdemCompra.gerarRelatorio("Relatorios/OrdemCompra/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioOrdemCompra.gerarRelatorio("Relatorios/OrdemCompra/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
