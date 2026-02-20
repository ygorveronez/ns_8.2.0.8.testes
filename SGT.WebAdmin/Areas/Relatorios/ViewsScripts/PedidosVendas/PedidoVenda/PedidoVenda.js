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
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Produto.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Servico.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoPedidoVenda.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumStatusPedidoVenda.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioPedidosVendas, _gridPedidosVendas, _pesquisaPedidosVendas, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _tipoPedidoVenda = [
    { text: "Todos", value: 0 },
    { text: "Cotação", value: EnumTipoPedidoVenda.Cotacao },
    { text: "Pedido", value: EnumTipoPedidoVenda.Pedido },
    { text: "Ordem de Serviço", value: EnumTipoPedidoVenda.OrdemServico }
];

var PesquisaPedidosVendas = function () {
    this.NumeroInicial = PropertyEntity({ text: "Número Inicial: ", getType: typesKnockout.int });
    this.NumeroFinal = PropertyEntity({ text: "Número Final: ", getType: typesKnockout.int });
    this.NumeroInternoInicial = PropertyEntity({ text: "Número Interno Inicial: ", getType: typesKnockout.int });
    this.NumeroInternoFinal = PropertyEntity({ text: "Número Interno Final: ", getType: typesKnockout.int });

    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.DataEntregaInicial = PropertyEntity({ text: "Data Entrega Inicial: ", getType: typesKnockout.date });
    this.DataEntregaFinal = PropertyEntity({ text: "Data Entrega Final: ", getType: typesKnockout.date });

    this.Status = PropertyEntity({ val: ko.observable(EnumStatusPedidoVenda.Todos), options: EnumStatusPedidoVenda.obterOpcoesPesquisa(), def: EnumStatusPedidoVenda.Todos, text: "Status: " });
    this.Tipo = PropertyEntity({ val: ko.observable(0), options: _tipoPedidoVenda, def: 0, text: "Tipo: " });

    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa: ", idBtnSearch: guid(), visible: true });
    this.Vendedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Vendedor: ", idBtnSearch: guid(), visible: true });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo: ", idBtnSearch: guid(), visible: true });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto: ", idBtnSearch: guid(), visible: true });
    this.Servico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Serviço: ", idBtnSearch: guid(), visible: true });
    this.FornecedorServico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor Serviço: ", idBtnSearch: guid(), visible: true });
    this.FuncionarioServico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Funcionário Serviço: ", idBtnSearch: guid(), visible: true });

    this.ExibirItens = PropertyEntity({ text: "Exibir os Itens na impressão?", val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridPedidosVendas.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaPedidosVendas.Visible.visibleFade()) {
                _pesquisaPedidosVendas.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaPedidosVendas.Visible.visibleFade(true);
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

function loadRelatorioPedidosVendas() {

    _pesquisaPedidosVendas = new PesquisaPedidosVendas();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridPedidosVendas = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/PedidoVenda/Pesquisa", _pesquisaPedidosVendas, null, null, 10);
    _gridPedidosVendas.SetPermitirEdicaoColunas(true);

    _relatorioPedidosVendas = new RelatorioGlobal("Relatorios/PedidoVenda/BuscarDadosRelatorio", _gridPedidosVendas, function () {
        _relatorioPedidosVendas.loadRelatorio(function () {
            KoBindings(_pesquisaPedidosVendas, "knockoutPesquisaPedidosVendas");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaPedidosVendas");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaPedidosVendas");

            new BuscarClientes(_pesquisaPedidosVendas.Pessoa);
            new BuscarFuncionario(_pesquisaPedidosVendas.Vendedor);
            new BuscarClientes(_pesquisaPedidosVendas.FornecedorServico);
            new BuscarFuncionario(_pesquisaPedidosVendas.FuncionarioServico);
            new BuscarVeiculos(_pesquisaPedidosVendas.Veiculo);
            new BuscarProdutoTMS(_pesquisaPedidosVendas.Produto);
            new BuscarServicoTMS(_pesquisaPedidosVendas.Servico);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaPedidosVendas);

}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioPedidosVendas.gerarRelatorio("Relatorios/PedidoVenda/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioPedidosVendas.gerarRelatorio("Relatorios/PedidoVenda/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}