/// <reference path="../../../../../ViewsScripts/Consultas/CentroCarregamento.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Carga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Pedido.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Produto.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoCargaJanelaCarregamento.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDRelatorio;
var _CRUDFiltrosRelatorio;
var _gridCargaProduto;
var _pesquisaCargaProduto;
var _relatorioCargaProduto;

/*
 * Declaração das Classes
 */

var PesquisaCargaProduto = function () {
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Carga:"), idBtnSearch: guid() });
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Centro de Carregamento:"), idBtnSearch: guid() });
    this.DataInicio = PropertyEntity({ text: "Data Início: ", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ text: "Data Limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid() });
    this.Pedido = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Pedido:"), idBtnSearch: guid() });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Produto:"), idBtnSearch: guid() });
    this.Situacao = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, params: { Tipo: "", Ativo: EnumSituacaoCargaJanelaCarregamento.Todas, OpcaoSemGrupo: false }, text: "Situação Janela Carregamento: ", options: ko.observable(EnumSituacaoCargaJanelaCarregamento.obterOpcoesPesquisaRelatorio()), visible: ko.observable(true) });

    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Transportador:"), idBtnSearch: guid() });
    this.ExibirCodigoBarras = PropertyEntity({ text: "Exibir o código de barras no agrupamento", getType: typesKnockout.bool, def: false, val: ko.observable(false) });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });

    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridCargaProduto.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaCargaProduto.Visible.visibleFade()) {
                _pesquisaCargaProduto.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaCargaProduto.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

/*
 * Declaração das Funções de Inicialização
 */

function LoadCargaProduto() {
    _pesquisaCargaProduto = new PesquisaCargaProduto();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();
    _gridCargaProduto = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/CargaProduto/Pesquisa", _pesquisaCargaProduto);

    _gridCargaProduto.SetPermitirEdicaoColunas(true);
    _gridCargaProduto.SetQuantidadeLinhasPorPagina(20);

    _relatorioCargaProduto = new RelatorioGlobal("Relatorios/CargaProduto/BuscarDadosRelatorio", _gridCargaProduto, function () {
        _relatorioCargaProduto.loadRelatorio(function () {
            KoBindings(_pesquisaCargaProduto, "knockoutPesquisaCargaProduto", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaCargaProduto", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaCargaProduto", false);

            new BuscarCargas(_pesquisaCargaProduto.Carga);
            new BuscarCentrosCarregamento(_pesquisaCargaProduto.CentroCarregamento);
            new BuscarClientes(_pesquisaCargaProduto.Destinatario);
            new BuscarPedidos(_pesquisaCargaProduto.Pedido, retornoConsultaPedido);
            new BuscarProdutos(_pesquisaCargaProduto.Produto);
            new BuscarTransportadores(_pesquisaCargaProduto.Transportador);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaCargaProduto);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function GerarRelatorioPDFClick() {
    _relatorioCargaProduto.gerarRelatorio("Relatorios/CargaProduto/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick() {
    _relatorioCargaProduto.gerarRelatorio("Relatorios/CargaProduto/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}

/*
 * Declaração das Funções
 */

function retornoConsultaPedido(pedidoSelecionado) {
    _pesquisaCargaProduto.Pedido.codEntity(pedidoSelecionado.Codigo);
    _pesquisaCargaProduto.Pedido.val(pedidoSelecionado.NumeroPedidoEmbarcador);
}