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
/// <reference path="../../../../../ViewsScripts/Consultas/Titulo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Fatura.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoFuncionarioComissao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioFuncionarioComissao, _gridFuncionarioComissao, _pesquisaFuncionarioComissao, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _situacaoRelatorioFuncionarioComissao = [
    { text: "Todos", value: "" },
    { text: "Ag. Aprovação", value: EnumSituacaoFuncionarioComissao.AgAprovacao },
    { text: "Aprovada", value: EnumSituacaoFuncionarioComissao.Aprovada },
    { text: "Cancelado", value: EnumSituacaoFuncionarioComissao.Cancelado },
    { text: "Finalizado", value: EnumSituacaoFuncionarioComissao.Finalizado },
    { text: "Rejeitada", value: EnumSituacaoFuncionarioComissao.Rejeitada },
    { text: "Sem Regra", value: EnumSituacaoFuncionarioComissao.SemRegra }
];

var PesquisaFuncionarioComissao = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date });
    this.NumeroInicial = PropertyEntity({ text: "Número Inicial:", getType: typesKnockout.int });
    this.NumeroFinal = PropertyEntity({ text: "Número Final:", getType: typesKnockout.int });

    this.Funcionario = PropertyEntity({ text: "Vendedor:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), val: ko.observable("") });
    this.Operador = PropertyEntity({ text: "Operador:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), val: ko.observable("") });
    this.Titulo = PropertyEntity({ text: "Título:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), val: ko.observable("") });
    this.Fatura = PropertyEntity({ text: "Fatura:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), val: ko.observable("") });

    this.Situacao = PropertyEntity({ val: ko.observable(""), options: _situacaoRelatorioFuncionarioComissao, def: "", text: "Situação: " });
    this.ExibirTitulos = PropertyEntity({ text: "Exibir Títulos vinculados as Comissões?", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridFuncionarioComissao.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******


function loadRelatorioFuncionarioComissao() {

    _pesquisaFuncionarioComissao = new PesquisaFuncionarioComissao();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridFuncionarioComissao = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/FuncionarioComissao/Pesquisa", _pesquisaFuncionarioComissao, null, null, 10);
    _gridFuncionarioComissao.SetPermitirEdicaoColunas(true);

    _relatorioFuncionarioComissao = new RelatorioGlobal("Relatorios/FuncionarioComissao/BuscarDadosRelatorio", _gridFuncionarioComissao, function () {
        _relatorioFuncionarioComissao.loadRelatorio(function () {
            KoBindings(_pesquisaFuncionarioComissao, "knockoutPesquisaFuncionarioComissao");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaFuncionarioComissao");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaFuncionarioComissao");

            new BuscarFuncionario(_pesquisaFuncionarioComissao.Funcionario);
            new BuscarFuncionario(_pesquisaFuncionarioComissao.Operador);
            new BuscarTitulo(_pesquisaFuncionarioComissao.Titulo, null, null, RetornoTitulo);
            new BuscarFatura(_pesquisaFuncionarioComissao.Fatura, RetornoFatura);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaFuncionarioComissao);
}

function RetornoTitulo(data) {
    _pesquisaFuncionarioComissao.Titulo.val(data.Codigo);
    _pesquisaFuncionarioComissao.Titulo.codEntity(data.Codigo);
}

function RetornoFatura(data) {
    _pesquisaFuncionarioComissao.Fatura.codEntity(data.Codigo);
    _pesquisaFuncionarioComissao.Fatura.val(data.Numero);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioFuncionarioComissao.gerarRelatorio("Relatorios/FuncionarioComissao/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioFuncionarioComissao.gerarRelatorio("Relatorios/FuncionarioComissao/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}