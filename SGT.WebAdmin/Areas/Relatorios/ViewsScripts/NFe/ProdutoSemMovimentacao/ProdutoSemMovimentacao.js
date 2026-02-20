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

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioProdutosSemMovimentacoes, _gridProdutosSemMovimentacoes, _pesquisaProdutosSemMovimentacoes, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaProdutosSemMovimentacoes = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });

    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto: ", idBtnSearch: guid(), visible: true });
    this.GrupoProduto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Produto:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.EstoqueDiferenteZero = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Estoque diferente de zero?", def: false });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridProdutosSemMovimentacoes.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******


function loadRelatorioProdutosSemMovimentacoes() {

    _pesquisaProdutosSemMovimentacoes = new PesquisaProdutosSemMovimentacoes();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridProdutosSemMovimentacoes = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ProdutoSemMovimentacao/Pesquisa", _pesquisaProdutosSemMovimentacoes, null, null, 10);
    _gridProdutosSemMovimentacoes.SetPermitirEdicaoColunas(true);

    _relatorioProdutosSemMovimentacoes = new RelatorioGlobal("Relatorios/ProdutoSemMovimentacao/BuscarDadosRelatorio", _gridProdutosSemMovimentacoes, function () {
        _relatorioProdutosSemMovimentacoes.loadRelatorio(function () {
            KoBindings(_pesquisaProdutosSemMovimentacoes, "knockoutPesquisaProdutosSemMovimentacoes");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaProdutosSemMovimentacoes");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaProdutosSemMovimentacoes");

            new BuscarProdutoTMS(_pesquisaProdutosSemMovimentacoes.Produto);
            new BuscarEmpresa(_pesquisaProdutosSemMovimentacoes.Empresa);
            new BuscarGruposProdutosTMS(_pesquisaProdutosSemMovimentacoes.GrupoProduto, null);

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiNFe)
                _pesquisaProdutosSemMovimentacoes.Empresa.visible(false);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaProdutosSemMovimentacoes);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioProdutosSemMovimentacoes.gerarRelatorio("Relatorios/ProdutoSemMovimentacao/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioProdutosSemMovimentacoes.gerarRelatorio("Relatorios/ProdutoSemMovimentacao/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}