/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Conhecimento.js" />
// <reference path="../../../../../ViewsScripts/Consultas/Fatura.js" />DescontoAcrescimoCTe
/// <reference path="../../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../../js/Global/CRUD.js" />
/// <reference path="../../../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../../../js/Global/Rest.js" />
/// <reference path="../../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../../js/Global/Grid.js" />
/// <reference path="../../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioFaturamento, _gridFaturamento, _pesquisaFaturamento, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _statusFaturamento = [
    { text: "Todos", value: 0 },
    { text: "Em Aberto", value: 1 },
    { text: "Quitado", value: 3 }
];

var _situacaoFatura = [
    { text: "Exceto Canceladas", value: -1 },
    { text: "Todos", value: 0 },
    { text: "Em Andamento", value: 1 },
    { text: "Fechado", value: 2 },
    { text: "Cancelado", value: 3 },
    { text: "Liquidado", value: 4 }
];

var PesquisaFaturamento = function () {
    this.ConhecimentoDeTransporte = PropertyEntity({ text: "CT-e:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), val: ko.observable(""), });
    this.Fatura = PropertyEntity({ text: "Fatura:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), val: ko.observable(""), });
    this.Pessoa = PropertyEntity({ text: "Cliente:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), val: ko.observable(""), });

    this.GrupoPessoa = PropertyEntity({ text: "Grupo de Pessoa:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), val: ko.observable(""), });
    this.DataInicialEmissaoCTe = PropertyEntity({ text: "Data de emissão do CT-e: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFinalEmissaoCTe = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataInicialEmissao = PropertyEntity({ text: "Data da Fatura: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFinalEmissao = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });

    this.DataInicialEmissaoFatura = PropertyEntity({ text: "Data de emissão da Fatura: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFinalEmissaoFatura = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataInicialVencimento = PropertyEntity({ text: "Data de vencimento do Título: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFinalVencimento = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataInicialQuitacao = PropertyEntity({ text: "Data da quitação do Título: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFinalQuitacao = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });

    this.ValorInicial = PropertyEntity({ text: "Valor inical da fatura: ", val: ko.observable("0,00"), getType: typesKnockout.decimal });
    this.ValorFinal = PropertyEntity({ text: "Até: ", val: ko.observable("0,00"), getType: typesKnockout.decimal });
    this.StatusFinanceiro = PropertyEntity({ val: ko.observable(0), options: _statusFaturamento, def: 0, text: "Status Financeiro: ", required: false });
    this.SituacaoFatura = PropertyEntity({ val: ko.observable(-1), options: _situacaoFatura, def: -1, text: "Situação Fatura: ", required: false });

    this.DataBaseInicial = PropertyEntity({ text: "Data base da quitação: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataBaseFinal = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true), val: ko.observable("") });
    this.GruposPessoas = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, url: "GrupoPessoas/ObterTodos", params: { Tipo: 0, Ativo: _statusPesquisa.Todos, OpcaoSemGrupo: true }, text: "Grupo de Pessoas: ", options: ko.observable(new Array()), issue: 58, visible: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-3 col-lg-3") });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridFaturamento.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function loadRelatorioFaturamento() {

    _pesquisaFaturamento = new PesquisaFaturamento();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridFaturamento = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/Faturamento/Pesquisa", _pesquisaFaturamento, null, null, 10);
    _gridFaturamento.SetPermitirEdicaoColunas(true);

    _relatorioFaturamento = new RelatorioGlobal("Relatorios/Faturamento/BuscarDadosRelatorio", _gridFaturamento, function () {
        _relatorioFaturamento.loadRelatorio(function () {
            KoBindings(_pesquisaFaturamento, "knockoutPesquisaFaturamento");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaFaturamento");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaFaturamento");

            new BuscarClientes(_pesquisaFaturamento.Pessoa);
            new BuscarGruposPessoas(_pesquisaFaturamento.GrupoPessoa, null, null, null, EnumTipoGrupoPessoas.Clientes);
            new BuscarConhecimentoNotaReferencia(_pesquisaFaturamento.ConhecimentoDeTransporte, RetornoCTe);
            new BuscarFatura(_pesquisaFaturamento.Fatura, RetornoFatura);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaFaturamento);

}

function RetornoCTe(data) {
    _pesquisaFaturamento.ConhecimentoDeTransporte.val(data.Numero + "-" + data.Serie);
    _pesquisaFaturamento.ConhecimentoDeTransporte.codEntity(data.Codigo);
}

function RetornoFatura(data) {
    _pesquisaFaturamento.Fatura.val(data.Numero);
    _pesquisaFaturamento.Fatura.codEntity(data.Codigo);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioFaturamento.gerarRelatorio("Relatorios/Faturamento/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioFaturamento.gerarRelatorio("Relatorios/Faturamento/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
