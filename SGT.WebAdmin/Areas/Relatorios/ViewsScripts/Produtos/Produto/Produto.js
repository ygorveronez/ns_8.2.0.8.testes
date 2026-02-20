/// <reference path="../../../../../ViewsScripts/Consultas/GrupoProdutoTMS.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/MarcaProduto.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/LocalArmazenamentoProduto.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Produto.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoImposto.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumCategoriaProduto.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioProduto, _gridProduto, _pesquisaProduto, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaProduto = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.CodigoNCM = PropertyEntity({ text: "Código NCM: ", maxlength: 8 });
    this.CodigoCEST = PropertyEntity({ text: "Código CEST: ", maxlength: 7 });
    this.CodigoBarrasEAN = PropertyEntity({ text: "Código EAN: ", maxlength: 50 });
    this.CategoriaProduto = PropertyEntity({ val: ko.observable(EnumCategoriaProduto.Todos), options: EnumCategoriaProduto.obterOpcoesPesquisa(), def: EnumCategoriaProduto.Todos, text: "Categoria Produto: " });
    this.Status = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });

    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto:", idBtnSearch: guid() });
    this.Grupo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Produto:", idBtnSearch: guid() });
    this.Marca = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Marca:", idBtnSearch: guid() });
    this.LocalArmazenamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Local de Armazenamento:", idBtnSearch: guid() });
    this.GrupoImposto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Imposto:", idBtnSearch: guid() });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridProduto.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarEtiqueta = PropertyEntity({ eventClick: GerarEtiquetaClik, type: types.event, text: "Gerar Etiqueta" });
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function LoadProduto() {
    _pesquisaProduto = new PesquisaProduto();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridProduto = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/Produto/Pesquisa", _pesquisaProduto, null, null, 10);
    _gridProduto.SetPermitirEdicaoColunas(true);

    _relatorioProduto = new RelatorioGlobal("Relatorios/Produto/BuscarDadosRelatorio", _gridProduto, function () {
        _relatorioProduto.loadRelatorio(function () {
            KoBindings(_pesquisaProduto, "knockoutPesquisaProduto", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaProduto", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaProduto", false);

            new BuscarProdutoTMS(_pesquisaProduto.Produto);
            new BuscarGruposProdutosTMS(_pesquisaProduto.Grupo);
            new BuscarMarcaProduto(_pesquisaProduto.Marca);
            new BuscarLocalArmazenamentoProduto(_pesquisaProduto.LocalArmazenamento);
            new BuscarGrupoImposto(_pesquisaProduto.GrupoImposto);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaProduto);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioProduto.gerarRelatorio("Relatorios/Produto/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioProduto.gerarRelatorio("Relatorios/Produto/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}

function GerarEtiquetaClik(e, sender) {
    executarDownloadArquivo("Relatorios/Produto/GerarEtiquetas", RetornarObjetoPesquisa(_pesquisaProduto));
}