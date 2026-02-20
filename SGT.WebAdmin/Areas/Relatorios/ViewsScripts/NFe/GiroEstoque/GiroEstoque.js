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

var _relatorioGirosEstoques, _gridGirosEstoques, _pesquisaGirosEstoques, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaGirosEstoques = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto: ", idBtnSearch: guid(), visible: true });
    this.GrupoProduto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Produto:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridGirosEstoques.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function loadRelatorioGirosEstoques() {

    _pesquisaGirosEstoques = new PesquisaGirosEstoques();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridGirosEstoques = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/GiroEstoque/Pesquisa", _pesquisaGirosEstoques, null, null, 10);
    _gridGirosEstoques.SetPermitirEdicaoColunas(true);

    _relatorioGirosEstoques = new RelatorioGlobal("Relatorios/GiroEstoque/BuscarDadosRelatorio", _gridGirosEstoques, function () {
        _relatorioGirosEstoques.loadRelatorio(function () {
            KoBindings(_pesquisaGirosEstoques, "knockoutPesquisaGirosEstoques");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaGirosEstoques");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaGirosEstoques");

            new BuscarProdutoTMS(_pesquisaGirosEstoques.Produto);
            new BuscarEmpresa(_pesquisaGirosEstoques.Empresa);
            new BuscarGruposProdutosTMS(_pesquisaGirosEstoques.GrupoProduto, null);

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiNFe)
                _pesquisaGirosEstoques.Empresa.visible(false);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaGirosEstoques);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioGirosEstoques.gerarRelatorio("Relatorios/GiroEstoque/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioGirosEstoques.gerarRelatorio("Relatorios/GiroEstoque/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}