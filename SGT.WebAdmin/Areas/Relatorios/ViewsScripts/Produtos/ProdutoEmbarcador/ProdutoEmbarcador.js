/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CanalEntrega.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoProduto.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoDeCarga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Pedido.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoOperacao.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Produto.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoPessoa.js" />

var _pesquisaProdutoEmbarcador;
var _gridPedidoEmbarcador;

var _statusPedidos = [
    { value: 1, text: "Faturado" },
    { value: 2, text: "Em Aberto" },
    { value: 3, text: "Em Carregamento" },
    { value: 99, text: "Cancelado" }
];

var PesquisaProdutoEmbarcador = function () {

    var dataInicial = moment().add(-1, 'days').format("DD/MM/YYYY");
    var dataFinal = moment().format("DD/MM/YYYY");

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.DataPedidoInicial = PropertyEntity({ text: "Data Pedido Inicial: ", getType: typesKnockout.date, val: ko.observable(dataInicial) });
    this.DataPedidoFinal = PropertyEntity({ text: "Data Pedido Final: ", getType: typesKnockout.date, val: ko.observable(dataFinal) });

    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable("Filial:"), idBtnSearch: guid() });
    this.CanalEntrega = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable("Canal Entrega:"), idBtnSearch: guid() });
    this.TipoCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable("Tipo de Carga:"), idBtnSearch: guid() });
    this.GrupoProduto = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable("Grupo de Produto:"), idBtnSearch: guid() });
    this.Destinatario = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable("Destinatário:"), idBtnSearch: guid() });
    this.Pedido = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable("Pedido Embarcador:"), idBtnSearch: guid() });
    this.StatusPedido = PropertyEntity({ val: ko.observable(new Array()), getType: typesKnockout.selectMultiple, options: _statusPedidos, def: new Array(), text: "Status Pedido:" });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable("Tipo Operação:"), idBtnSearch: guid() });
    this.Produto = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable("Produto:"), idBtnSearch: guid() });
    this.GrupoPessoa = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable("Grupo Pessoa:"), idBtnSearch: guid() });

    this.DataPedidoInicial.dateRangeLimit = this.DataPedidoFinal;
    this.DataPedidoFinal.dateRangeInit = this.DataPedidoInicial;
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridPedidoEmbarcador.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaPedido.Visible.visibleFade()) {
                _pesquisaPedido.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaPedido.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

function LoadProdutoEmbarcador() {
    
    _pesquisaProdutoEmbarcador = new PesquisaProdutoEmbarcador();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridPedidoEmbarcador = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "/Relatorios/ProdutoEmbarcador/Pesquisa", _pesquisaProdutoEmbarcador, null, null, 10, null, null, null, null, 20);
    _gridPedidoEmbarcador.SetPermitirEdicaoColunas(true);

    _relatorioPedido = new RelatorioGlobal("Relatorios/ProdutoEmbarcador/BuscarDadosRelatorio", _gridPedidoEmbarcador, function () {
        _relatorioPedido.loadRelatorio(function () {
            KoBindings(_pesquisaProdutoEmbarcador, "knockoutPesquisaProdutoEmbarcador");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaProdutoEmbarcador");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaProdutoEmbarcador");

            new BuscarTipoDeCargaDoPedido(_pesquisaProdutoEmbarcador.TipoCarga);
            new BuscarFilial(_pesquisaProdutoEmbarcador.Filial);
            new BuscarClientes(_pesquisaProdutoEmbarcador.Destinatario);
            new BuscarPedidos(_pesquisaProdutoEmbarcador.Pedido, null, null, true);
            new BuscarCanaisEntrega(_pesquisaProdutoEmbarcador.CanalEntrega);
            new BuscarGruposProdutos(_pesquisaProdutoEmbarcador.GrupoProduto);
            new BuscarTiposOperacao(_pesquisaProdutoEmbarcador.TipoOperacao);
            new BuscarProdutos(_pesquisaProdutoEmbarcador.Produto);
            new BuscarGruposPessoas(_pesquisaProdutoEmbarcador.GrupoPessoa);
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaProdutoEmbarcador);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioPedido.gerarRelatorio("Relatorios/ProdutoEmbarcador/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioPedido.gerarRelatorio("Relatorios/ProdutoEmbarcador/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}