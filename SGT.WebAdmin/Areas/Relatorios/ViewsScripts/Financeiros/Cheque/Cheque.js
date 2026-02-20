/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Titulo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Banco.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumStatusCheque.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoCheque.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDRelatorio;
var _CRUDFiltrosRelatorio;
var _gridCheque;
var _pesquisaCheque;
var _relatorioCheque;

/*
 * Declaração das Classes
 */

var PesquisaCheque = function () {
    this.DataCompensacaoInicio = PropertyEntity({ text: "Data Compensação Início: ", getType: typesKnockout.date });
    this.DataCompensacaoLimite = PropertyEntity({ text: "Data Compensação Limite: ", dateRangeInit: this.DataCompensacaoInicio, getType: typesKnockout.date });
    this.DataTransacaoInicio = PropertyEntity({ text: "Data Transação Início: ", getType: typesKnockout.date });
    this.DataTransacaoLimite = PropertyEntity({ text: "Data Transação Limite: ", dateRangeInit: this.DataTransacaoInicio, getType: typesKnockout.date });
    this.DataVencimentoInicio = PropertyEntity({ text: "Data Vencimento Início: ", getType: typesKnockout.date });
    this.DataVencimentoLimite = PropertyEntity({ text: "Data Vencimento Limite: ", dateRangeInit: this.DataVencimentoInicio, getType: typesKnockout.date });
    this.NumeroCheque = PropertyEntity({ text: "Nº Cheque: " });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Pessoa:"), idBtnSearch: guid() });
    this.Status = PropertyEntity({ val: ko.observable(EnumStatusCheque.Todos), options: EnumStatusCheque.obterOpcoesPesquisa(), def: EnumStatusCheque.Todos, text: "Status: " });
    this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoCheque.Todos), options: EnumTipoCheque.obterOpcoesPesquisa(), def: EnumTipoCheque.Todos, text: "Tipo: " });
    this.Titulo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Título:"), idBtnSearch: guid() });
    this.ValorInicio = PropertyEntity({ getType: typesKnockout.decimal, text: "Valor Início:" });
    this.ValorLimite = PropertyEntity({ getType: typesKnockout.decimal, text: "Valor Limite:" });
    this.Banco = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Banco:"), idBtnSearch: guid() });

    this.DataCompensacaoInicio.dateRangeLimit = this.DataCompensacaoLimite;
    this.DataCompensacaoLimite.dateRangeInit = this.DataCompensacaoInicio;
    this.DataTransacaoInicio.dateRangeLimit = this.DataTransacaoLimite;
    this.DataTransacaoLimite.dateRangeInit = this.DataTransacaoInicio;
    this.DataVencimentoInicio.dateRangeLimit = this.DataVencimentoLimite;
    this.DataVencimentoLimite.dateRangeInit = this.DataVencimentoInicio;

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true), val: ko.observable("") });
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridCheque.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaCheque.Visible.visibleFade()) {
                _pesquisaCheque.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaCheque.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

/*
 * Declaração das Funções de Inicialização
 */

function LoadCheque() {
    _pesquisaCheque = new PesquisaCheque();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();
    _gridCheque = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/Cheque/Pesquisa", _pesquisaCheque);

    _gridCheque.SetPermitirEdicaoColunas(true);
    _gridCheque.SetQuantidadeLinhasPorPagina(20);

    _relatorioCheque = new RelatorioGlobal("Relatorios/Cheque/BuscarDadosRelatorio", _gridCheque, function () {
        _relatorioCheque.loadRelatorio(function () {
            KoBindings(_pesquisaCheque, "knockoutPesquisaCheque", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaCheque", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaCheque", false);

            new BuscarClientes(_pesquisaCheque.Pessoa);
            new BuscarTitulo(_pesquisaCheque.Titulo, null, null, retornoTitulo);
            new BuscarBanco(_pesquisaCheque.Banco);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaCheque);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function GerarRelatorioPDFClick() {
    _relatorioCheque.gerarRelatorio("Relatorios/Cheque/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick() {
    _relatorioCheque.gerarRelatorio("Relatorios/Cheque/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}

function retornoTitulo(tituloSelecionado) {
    _pesquisaCheque.Titulo.val(tituloSelecionado.Codigo);
    _pesquisaCheque.Titulo.codEntity(tituloSelecionado.Codigo);
}