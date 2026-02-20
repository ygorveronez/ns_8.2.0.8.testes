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
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumEntradaSaida.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Produto.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioHistoricoProdutos, _gridHistoricoProdutos, _pesquisaHistoricoProdutos, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _tipoEntradaSaida = [
    { text: "Todos", value: EnumEntradaSaida.Todos },
    { text: "Entrada", value: EnumEntradaSaida.Entrada },
    { text: "Saida", value: EnumEntradaSaida.Saida }
];

var PesquisaHistoricoProdutos = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });

    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto: ", idBtnSearch: guid(), visible: true });
    this.GrupoProduto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Produto:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });

    this.TipoMovimento = PropertyEntity({ val: ko.observable(EnumEntradaSaida.Todos), options: _tipoEntradaSaida, def: EnumEntradaSaida.Todos, text: "Tipo Movimento: " });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridHistoricoProdutos.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******


function loadRelatorioHistoricoProdutos() {

    _pesquisaHistoricoProdutos = new PesquisaHistoricoProdutos();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridHistoricoProdutos = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/HistoricoProduto/Pesquisa", _pesquisaHistoricoProdutos, null, null, 10);
    _gridHistoricoProdutos.SetPermitirEdicaoColunas(true);

    _relatorioHistoricoProdutos = new RelatorioGlobal("Relatorios/HistoricoProduto/BuscarDadosRelatorio", _gridHistoricoProdutos, function () {
        _relatorioHistoricoProdutos.loadRelatorio(function () {
            KoBindings(_pesquisaHistoricoProdutos, "knockoutPesquisaHistoricoProdutos");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaHistoricoProdutos");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaHistoricoProdutos");

            new BuscarProdutoTMS(_pesquisaHistoricoProdutos.Produto);
            new BuscarEmpresa(_pesquisaHistoricoProdutos.Empresa);
            new BuscarGruposProdutosTMS(_pesquisaHistoricoProdutos.GrupoProduto, null);

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiNFe)
                _pesquisaHistoricoProdutos.Empresa.visible(false);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaHistoricoProdutos);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioHistoricoProdutos.gerarRelatorio("Relatorios/HistoricoProduto/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioHistoricoProdutos.gerarRelatorio("Relatorios/HistoricoProduto/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}