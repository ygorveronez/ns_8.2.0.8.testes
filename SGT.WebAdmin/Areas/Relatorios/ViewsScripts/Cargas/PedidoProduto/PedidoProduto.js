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
/// <reference path="../../../../../ViewsScripts/Consultas/Produto.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioPedidoProduto, _gridPedidoProduto, _pesquisaPedidoProduto, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaPedidoProduto = function () {
    this.DataInicial = PropertyEntity({ text: "Data Coleta Inicial:", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Coleta Final:", getType: typesKnockout.date });

    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid() });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid() });
    this.Produto = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Produto:", idBtnSearch: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridPedidoProduto.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******


function loadRelatorioPedidoProduto() {

    _pesquisaPedidoProduto = new PesquisaPedidoProduto();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridPedidoProduto = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/PedidoProduto/Pesquisa", _pesquisaPedidoProduto, null, null, 10);
    _gridPedidoProduto.SetPermitirEdicaoColunas(true);

    _relatorioPedidoProduto = new RelatorioGlobal("Relatorios/PedidoProduto/BuscarDadosRelatorio", _gridPedidoProduto, function () {
        _relatorioPedidoProduto.loadRelatorio(function () {
            KoBindings(_pesquisaPedidoProduto, "knockoutPesquisaPedidoProduto");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaPedidoProduto");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaPedidoProduto");

            new BuscarClientes(_pesquisaPedidoProduto.Remetente);
            new BuscarClientes(_pesquisaPedidoProduto.Destinatario);
            new BuscarProdutos(_pesquisaPedidoProduto.Produto);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaPedidoProduto);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioPedidoProduto.gerarRelatorio("Relatorios/PedidoProduto/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioPedidoProduto.gerarRelatorio("Relatorios/PedidoProduto/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}