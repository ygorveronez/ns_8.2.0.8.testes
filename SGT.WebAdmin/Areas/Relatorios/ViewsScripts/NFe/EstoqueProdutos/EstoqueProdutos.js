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
/// <reference path="../../../../../ViewsScripts/Consultas/LocalArmazenamentoProduto.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/MarcaProduto.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumCategoriaProduto.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioEstoqueProdutos, _gridEstoqueProdutos, _pesquisaEstoqueProdutos, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _statusEstoqueProduto = [
    { text: "Todos", value: "T" },
    { text: "Ativo", value: "A" },
    { text: "Inativo", value: "I" }
];

var PesquisaEstoqueProdutos = function () {
    this.CodigoProduto = PropertyEntity({ text: "Cód. Produto: ", getType: typesKnockout.string });
    this.NCM = PropertyEntity({ text: "NCM: ", getType: typesKnockout.string });
    this.Descricao = PropertyEntity({ text: "Descrição: ", getType: typesKnockout.string, maxlength: 500 });
    this.Status = PropertyEntity({ val: ko.observable("T"), options: _statusEstoqueProduto, def: "T", text: "Status: " });
    this.Categoria = PropertyEntity({ val: ko.observable(EnumCategoriaProduto.Todos), options: EnumCategoriaProduto.obterOpcoesPesquisa(), def: EnumCategoriaProduto.Todos, text: "Categoria Produto: " });

    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto: ", idBtnSearch: guid() });
    this.GrupoProduto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Produto:", idBtnSearch: guid() });
    this.LocalArmazenamentoProduto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Local de Armazenamento:", idBtnSearch: guid() });
    this.MarcaProduto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Marca:", idBtnSearch: guid() });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa:", idBtnSearch: guid(), visible: ko.observable(true) });    
    this.DataPosicaoEstoque = PropertyEntity({ text: "Data Posição do Estoque: ", getType: typesKnockout.date });

    this.EstoqueReservado = PropertyEntity({ text: "Somente produtos com estoque reservado", getType: typesKnockout.bool });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridEstoqueProdutos.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******


function loadRelatorioEstoqueProdutos() {

    _pesquisaEstoqueProdutos = new PesquisaEstoqueProdutos();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridEstoqueProdutos = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/EstoqueProdutos/Pesquisa", _pesquisaEstoqueProdutos, null, null, 10);
    _gridEstoqueProdutos.SetPermitirEdicaoColunas(true);

    _relatorioEstoqueProdutos = new RelatorioGlobal("Relatorios/EstoqueProdutos/BuscarDadosRelatorio", _gridEstoqueProdutos, function () {
        _relatorioEstoqueProdutos.loadRelatorio(function () {
            KoBindings(_pesquisaEstoqueProdutos, "knockoutPesquisaEstoqueProdutos");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaEstoqueProdutos");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaEstoqueProdutos");

            new BuscarProdutoTMS(_pesquisaEstoqueProdutos.Produto);
            new BuscarEmpresa(_pesquisaEstoqueProdutos.Empresa);
            new BuscarGruposProdutosTMS(_pesquisaEstoqueProdutos.GrupoProduto, null);
            new BuscarLocalArmazenamentoProduto(_pesquisaEstoqueProdutos.LocalArmazenamentoProduto);
            new BuscarMarcaProduto(_pesquisaEstoqueProdutos.MarcaProduto);

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe)
                _pesquisaEstoqueProdutos.Empresa.visible(false);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaEstoqueProdutos);

}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioEstoqueProdutos.gerarRelatorio("Relatorios/EstoqueProdutos/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioEstoqueProdutos.gerarRelatorio("Relatorios/EstoqueProdutos/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
