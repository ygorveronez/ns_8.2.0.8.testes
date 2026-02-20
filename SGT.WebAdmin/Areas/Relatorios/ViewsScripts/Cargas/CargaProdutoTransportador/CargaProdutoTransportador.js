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
var _gridCargaProdutoTransportador;
var _pesquisaCargaProdutoTransportador;
var _relatorioCargaProdutoTransportador;

/*
 * Declaração das Classes
 */

var PesquisaCargaProdutoTransportador = function () {
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
            _gridCargaProdutoTransportador.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaCargaProdutoTransportador.Visible.visibleFade()) {
                _pesquisaCargaProdutoTransportador.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaCargaProdutoTransportador.Visible.visibleFade(true);
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

function LoadCargaProdutoTransportador() {
    _pesquisaCargaProdutoTransportador = new PesquisaCargaProdutoTransportador();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();
    _gridCargaProdutoTransportador = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/CargaProdutoTransportador/Pesquisa", _pesquisaCargaProdutoTransportador);

    _gridCargaProdutoTransportador.SetPermitirEdicaoColunas(true);
    _gridCargaProdutoTransportador.SetQuantidadeLinhasPorPagina(20);

    _relatorioCargaProdutoTransportador = new RelatorioGlobal("Relatorios/CargaProdutoTransportador/BuscarDadosRelatorio", _gridCargaProdutoTransportador, function () {
        _relatorioCargaProdutoTransportador.loadRelatorio(function () {
            KoBindings(_pesquisaCargaProdutoTransportador, "knockoutPesquisaCargaProdutoTransportador", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaCargaProdutoTransportador", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaCargaProdutoTransportador", false);

            new BuscarCargas(_pesquisaCargaProdutoTransportador.Carga);
            new BuscarCentrosCarregamento(_pesquisaCargaProdutoTransportador.CentroCarregamento);
            new BuscarClientes(_pesquisaCargaProdutoTransportador.Destinatario);
            new BuscarPedidos(_pesquisaCargaProdutoTransportador.Pedido, retornoConsultaPedido);
            new BuscarProdutos(_pesquisaCargaProdutoTransportador.Produto);
            new BuscarTransportadores(_pesquisaCargaProdutoTransportador.Transportador);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaCargaProdutoTransportador);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function GerarRelatorioPDFClick() {
    _relatorioCargaProdutoTransportador.gerarRelatorio("Relatorios/CargaProdutoTransportador/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick() {
    _relatorioCargaProdutoTransportador.gerarRelatorio("Relatorios/CargaProdutoTransportador/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}

/*
 * Declaração das Funções
 */

function retornoConsultaPedido(pedidoSelecionado) {
    _pesquisaCargaProdutoTransportador.Pedido.codEntity(pedidoSelecionado.Codigo);
    _pesquisaCargaProdutoTransportador.Pedido.val(pedidoSelecionado.NumeroPedidoEmbarcador);
}