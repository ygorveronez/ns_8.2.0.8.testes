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
/// <reference path="../../../../../ViewsScripts/Consultas/Produto.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoProdutoTMS.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/LocalArmazenamentoProduto.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumCategoriaProduto.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioHistoricoEstoque, _gridHistoricoEstoque, _pesquisaHistoricoEstoque, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _statusHistoricoEstoque = [
    { text: "Todos", value: "T" },
    { text: "Ativo", value: "A" },
    { text: "Inativo", value: "I" }
];

var PesquisaHistoricoEstoque = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Status = PropertyEntity({ val: ko.observable("T"), options: _statusHistoricoEstoque, def: "T", text: "Status: " });
    this.Categoria = PropertyEntity({ val: ko.observable(EnumCategoriaProduto.Todos), options: EnumCategoriaProduto.obterOpcoesPesquisa(), def: EnumCategoriaProduto.Todos, text: "Categoria Produto: " });

    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto: ", idBtnSearch: guid() });
    this.GrupoProduto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Produto:", idBtnSearch: guid() });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.LocalArmazenamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Local de Armazenamento:", idBtnSearch: guid(), visible: ko.observable(false) });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridHistoricoEstoque.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******


function loadRelatorioHistoricoEstoque() {

    _pesquisaHistoricoEstoque = new PesquisaHistoricoEstoque();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridHistoricoEstoque = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/HistoricoEstoque/Pesquisa", _pesquisaHistoricoEstoque, null, null, 10);
    _gridHistoricoEstoque.SetPermitirEdicaoColunas(true);

    _relatorioHistoricoEstoque = new RelatorioGlobal("Relatorios/HistoricoEstoque/BuscarDadosRelatorio", _gridHistoricoEstoque, function () {
        _relatorioHistoricoEstoque.loadRelatorio(function () {
            KoBindings(_pesquisaHistoricoEstoque, "knockoutPesquisaHistoricoEstoque");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaHistoricoEstoque");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaHistoricoEstoque");

            new BuscarProdutoTMS(_pesquisaHistoricoEstoque.Produto);
            new BuscarEmpresa(_pesquisaHistoricoEstoque.Empresa);
            new BuscarGruposProdutosTMS(_pesquisaHistoricoEstoque.GrupoProduto, null);
            new BuscarLocalArmazenamentoProduto(_pesquisaHistoricoEstoque.LocalArmazenamento);

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe)
                _pesquisaHistoricoEstoque.Empresa.visible(false);

            if (_CONFIGURACAO_TMS.UtilizaMultiplosLocaisArmazenamento)
                _pesquisaHistoricoEstoque.LocalArmazenamento.visible(true);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaHistoricoEstoque);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioHistoricoEstoque.gerarRelatorio("Relatorios/HistoricoEstoque/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioHistoricoEstoque.gerarRelatorio("Relatorios/HistoricoEstoque/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
