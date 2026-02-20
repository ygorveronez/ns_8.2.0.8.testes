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
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumCompraVenda.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumOrdenacaoCurvaABC.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioCurvaABCProdutos, _gridCurvaABCProdutos, _pesquisaCurvaABCProdutos, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _tipoCompraVenda = [
    { text: "Compra", value: EnumCompraVenda.Compra },
    { text: "Venda", value: EnumCompraVenda.Venda }
];

var _ordemCurvaABC = [
    { text: "Quantidade", value: EnumOrdenacaoCurvaABC.Quantidade },
    { text: "Valor", value: EnumOrdenacaoCurvaABC.Valor },
    { text: "Vezes", value: EnumOrdenacaoCurvaABC.Vezes }
];

var PesquisaCurvaABCProdutos = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });

    this.TipoMovimento = PropertyEntity({ val: ko.observable(EnumCompraVenda.Venda), options: _tipoCompraVenda, def: EnumCompraVenda.Venda, text: "Tipo Movimento: " });
    this.Ordenar = PropertyEntity({ val: ko.observable(EnumOrdenacaoCurvaABC.Valor), options: _ordemCurvaABC, def: EnumOrdenacaoCurvaABC.Valor, text: "Ordenação: " });

    this.GrupoProduto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Produto:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridCurvaABCProdutos.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******


function loadRelatorioCurvaABCProdutos() {

    _pesquisaCurvaABCProdutos = new PesquisaCurvaABCProdutos();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridCurvaABCProdutos = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/CurvaABCProduto/Pesquisa", _pesquisaCurvaABCProdutos, null, null, 10);
    _gridCurvaABCProdutos.SetPermitirEdicaoColunas(true);

    _relatorioCurvaABCProdutos = new RelatorioGlobal("Relatorios/CurvaABCProduto/BuscarDadosRelatorio", _gridCurvaABCProdutos, function () {
        _relatorioCurvaABCProdutos.loadRelatorio(function () {
            KoBindings(_pesquisaCurvaABCProdutos, "knockoutPesquisaCurvaABCProdutos");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaCurvaABCProdutos");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaCurvaABCProdutos");

            new BuscarEmpresa(_pesquisaCurvaABCProdutos.Empresa);
            new BuscarGruposProdutosTMS(_pesquisaCurvaABCProdutos.GrupoProduto, null);

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiNFe)
                _pesquisaCurvaABCProdutos.Empresa.visible(false);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaCurvaABCProdutos);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioCurvaABCProdutos.gerarRelatorio("Relatorios/CurvaABCProduto/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioCurvaABCProdutos.gerarRelatorio("Relatorios/CurvaABCProduto/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}