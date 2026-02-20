/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CanalEntrega.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoProduto.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoCarga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Pedido.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoOperacao.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Produto.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoPessoa.js" />
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
/// <reference path="../../../../../js/app.config.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridCargaPedidoEmbarcador, _pesquisaCargaPedidoEmbarcador, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioCargaPedidoEmbarcador;

var _statusPedidos = [
    { value: 1, text: "Faturado" },
    { value: 2, text: "Em Aberto" },
    { value: 3, text: "Em Carregamento" }
];

var PesquisaCargaPedidoEmbarcador = function () {
    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.DataInicial = PropertyEntity({ text: "Data Pedido Inicial: ", required: true, getType: typesKnockout.date, def: (Global.DataAtual()) });
    this.DataFinal = PropertyEntity({ text: "Data Pedido Final: ", required: true, dateRangeInit: this.DataInicial, getType: typesKnockout.date, def: (Global.DataAtual()) });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable("Filial:"), idBtnSearch: guid() });
    this.CanalEntrega = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable("Canal Entrega:"), idBtnSearch: guid() });
    this.TipoCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable("Tipo de Carga:"), idBtnSearch: guid() });
    this.GrupoProduto = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable("Grupo de Produto:"), idBtnSearch: guid() });
    this.Destinatario = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable("Destinatário:"), idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable("Tipo Operação:"), idBtnSearch: guid() });
    this.Produto = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable("Produto:"), idBtnSearch: guid() });
    this.GrupoPessoa = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable("Grupo Pessoa:"), idBtnSearch: guid() });
    this.Pedido = PropertyEntity({ getType: typesKnockout.string, codEntity: ko.observable(0), text: ko.observable("Pedido Embarcador:"), idBtnSearch: guid() });
    this.StatusPedido = PropertyEntity({ val: ko.observable(new Array()), getType: typesKnockout.selectMultiple, options: _statusPedidos, def: new Array(), text: "Status Pedido:" });

};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridCargaPedidoEmbarcador.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    /*this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaCargaPedidoEmbarcador.Visible.visibleFade()) {
                _pesquisaCargaPedidoEmbarcador.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaCargaPedidoEmbarcador.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });*/
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function LoadCargaPedidoEmbarcador() {
    _pesquisaCargaPedidoEmbarcador = new PesquisaCargaPedidoEmbarcador();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridCargaPedidoEmbarcador = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/CargaPedidoEmbarcador/Pesquisa", _pesquisaCargaPedidoEmbarcador);

    _gridCargaPedidoEmbarcador.SetPermitirEdicaoColunas(true);
    _gridCargaPedidoEmbarcador.SetQuantidadeLinhasPorPagina(10);

    _relatorioCargaPedidoEmbarcador = new RelatorioGlobal("Relatorios/CargaPedidoEmbarcador/BuscarDadosRelatorio", _gridCargaPedidoEmbarcador, function () {
        _relatorioCargaPedidoEmbarcador.loadRelatorio(function () {
            KoBindings(_pesquisaCargaPedidoEmbarcador, "knockoutPesquisaCargaPedidoEmbarcador", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaCargaPedidoEmbarcador", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaCargaPedidoEmbarcador", false);

            new BuscarTiposdeCarga(_pesquisaCargaPedidoEmbarcador.TipoCarga);
            new BuscarFilial(_pesquisaCargaPedidoEmbarcador.Filial);
            new BuscarClientes(_pesquisaCargaPedidoEmbarcador.Destinatario);            
            new BuscarCanaisEntrega(_pesquisaCargaPedidoEmbarcador.CanalEntrega);
            new BuscarGruposProdutos(_pesquisaCargaPedidoEmbarcador.GrupoProduto);
            new BuscarTiposOperacao(_pesquisaCargaPedidoEmbarcador.TipoOperacao);
            new BuscarProdutos(_pesquisaCargaPedidoEmbarcador.Produto);
            new BuscarGruposPessoas(_pesquisaCargaPedidoEmbarcador.GrupoPessoa);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaCargaPedidoEmbarcador);

}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioCargaPedidoEmbarcador.gerarRelatorio("Relatorios/CargaPedidoEmbarcador/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioCargaPedidoEmbarcador.gerarRelatorio("Relatorios/CargaPedidoEmbarcador/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}

//*******MÉTODOS*******