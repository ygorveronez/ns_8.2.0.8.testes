/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoMovimento.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Equipamento.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/NaturezaOperacao.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/SegmentoVeiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Produto.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CentroResultado.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridManutencao, _pesquisaManutencao, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioManutencao;

var PesquisaManutencao = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.DataInicial = PropertyEntity({ text: "Período inicial de emissão da nota: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });
    this.TipoMovimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Movimento:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.NaturezaOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Natureza da Operação:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Segmento = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Segmentos do veículo:", idBtnSearch: guid(), issue: 0 });
    this.Equipamento = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Equipamentos:", idBtnSearch: guid(), issue: 0 });
    this.Veiculo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(""), defCodEntity: "", text: "Veículo:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor:", idBtnSearch: guid() });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.CentroResultado = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Centro de Resultado:", idBtnSearch: guid(), issue: 0 });
    this.Produto = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Produto:", idBtnSearch: guid(), issue: 0 });
    this.ExibirApenasComVeiculoOuEquipamento = PropertyEntity({ text: "Exibir apenas com Veículo/Equipamento", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.SituacaoLancDocEntrada = PropertyEntity({ val: ko.observable(EnumSituacaoDocumentoEntrada.Todos), options: EnumSituacaoDocumentoEntrada.obterOpcoesPesquisa(), def: EnumSituacaoDocumentoEntrada.Todos, text: "Situação Lanç. Doc. Entrada" });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridManutencao.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaManutencao.Visible.visibleFade()) {
                _pesquisaManutencao.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaManutencao.Visible.visibleFade(true);
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

function LoadManutencao() {
    _pesquisaManutencao = new PesquisaManutencao();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridManutencao = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/Manutencao/Pesquisa", _pesquisaManutencao);

    _gridManutencao.SetPermitirEdicaoColunas(true);
    _gridManutencao.SetQuantidadeLinhasPorPagina(10);

    _relatorioManutencao = new RelatorioGlobal("Relatorios/Manutencao/BuscarDadosRelatorio", _gridManutencao, function () {
        _relatorioManutencao.loadRelatorio(function () {
            KoBindings(_pesquisaManutencao, "knockoutPesquisaManutencao", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaManutencao", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaManutencao", false);

            new BuscarTipoMovimento(_pesquisaManutencao.TipoMovimento);
            new BuscarNaturezasOperacoesNotaFiscal(_pesquisaManutencao.NaturezaOperacao);
            new BuscarVeiculos(_pesquisaManutencao.Veiculo);
            new BuscarSegmentoVeiculo(_pesquisaManutencao.Segmento);
            new BuscarEquipamentos(_pesquisaManutencao.Equipamento);
            new BuscarClientes(_pesquisaManutencao.Fornecedor);
            new BuscarCentroResultado(_pesquisaManutencao.CentroResultado);
            new BuscarProdutoTMS(_pesquisaManutencao.Produto);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaManutencao);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioManutencao.gerarRelatorio("Relatorios/Manutencao/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioManutencao.gerarRelatorio("Relatorios/Manutencao/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
