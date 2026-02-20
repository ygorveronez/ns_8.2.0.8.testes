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
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoRequisicaoMercadoria.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumModoRequisicaoMercadoria.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioRequisicaoMercadoria, _gridRequisicaoMercadoria, _pesquisaRequisicaoMercadoria, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _tipoRequisicaoMercadoriaPesquisa = [
    { text: "Todos", value: EnumModoRequisicaoMercadoria.Todos },
    { text: "Compra", value: EnumModoRequisicaoMercadoria.Compra },
    { text: "Requisição", value: EnumModoRequisicaoMercadoria.Requisicao }
];

var _situacaoRequisicaoMercadoriaPesquisa = [
    { text: "Ag. Aprovação", value: EnumSituacaoRequisicaoMercadoria.AgAprovacao },
    { text: "Aprovada", value: EnumSituacaoRequisicaoMercadoria.Aprovada },
    { text: "Rejeitada", value: EnumSituacaoRequisicaoMercadoria.Rejeitada },
    { text: "Finalizado", value: EnumSituacaoRequisicaoMercadoria.Finalizado }
];

var PesquisaRequisicaoMercadoria = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.NumeroInicial = PropertyEntity({ text: "Número Inicial:", getType: typesKnockout.int });
    this.NumeroFinal = PropertyEntity({ text: "Número Final:", getType: typesKnockout.int });

    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.GrupoProduto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Produto:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.Colaborador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Colaborador:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.FuncionarioRequisitado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Funcionário Requisitado:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.Motivo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motivo:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });
    this.SetorFuncionario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Setor de funcionario:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });

    this.Situacao = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.selectMultiple, text: "Situação: ", options: _situacaoRequisicaoMercadoriaPesquisa, visible: ko.observable(true) });
    this.Tipo = PropertyEntity({ val: ko.observable(EnumModoRequisicaoMercadoria.Todos), options: _tipoRequisicaoMercadoriaPesquisa, def: EnumModoRequisicaoMercadoria.Todos, text: "Tipo:" });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridRequisicaoMercadoria.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******


function loadRelatorioRequisicaoMercadoria() {

    _pesquisaRequisicaoMercadoria = new PesquisaRequisicaoMercadoria();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridRequisicaoMercadoria = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/RequisicaoMercadoria/Pesquisa", _pesquisaRequisicaoMercadoria, null, null, 10);
    _gridRequisicaoMercadoria.SetPermitirEdicaoColunas(true);

    _relatorioRequisicaoMercadoria = new RelatorioGlobal("Relatorios/RequisicaoMercadoria/BuscarDadosRelatorio", _gridRequisicaoMercadoria, function () {
        _relatorioRequisicaoMercadoria.loadRelatorio(function () {
            KoBindings(_pesquisaRequisicaoMercadoria, "knockoutPesquisaRequisicaoMercadoria");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaRequisicaoMercadoria");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaRequisicaoMercadoria");

            new BuscarEmpresa(_pesquisaRequisicaoMercadoria.Empresa);
            new BuscarProdutoTMS(_pesquisaRequisicaoMercadoria.Produto);
            new BuscarGruposProdutosTMS(_pesquisaRequisicaoMercadoria.GrupoProduto, null);
            new BuscarFuncionario(_pesquisaRequisicaoMercadoria.Colaborador);
            new BuscarFuncionario(_pesquisaRequisicaoMercadoria.FuncionarioRequisitado);
            new BuscarMotivoCompra(_pesquisaRequisicaoMercadoria.Motivo);
            new BuscarSetorFuncionario(_pesquisaRequisicaoMercadoria.SetorFuncionario)

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiNFe) {
                _pesquisaRequisicaoMercadoria.Empresa.visible(true);
            }

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaRequisicaoMercadoria);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioRequisicaoMercadoria.gerarRelatorio("Relatorios/RequisicaoMercadoria/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioRequisicaoMercadoria.gerarRelatorio("Relatorios/RequisicaoMercadoria/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}