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

var _relatorioDescontoAcrescimoFatura, _gridDescontoAcrescimoFatura, _pesquisaDescontoAcrescimoFatura, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _statusDescontoAcrescimoFatura = [
    { text: "Todos", value: 0 },
    { text: "Em Aberto", value: 1 },
    { text: "Quitado", value: 3 }
];

var PesquisaDescontoAcrescimoFatura = function () {
    this.ConhecimentoDeTransporte = PropertyEntity({ text: "CT-e:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), val: ko.observable(""), });
    this.Fatura = PropertyEntity({ text: "Fatura:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), val: ko.observable(""), });
    this.Pessoa = PropertyEntity({ text: "Cliente:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), val: ko.observable(""), });
    this.GrupoPessoa = PropertyEntity({ text: "Grupo de Pessoa:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), val: ko.observable(""), });

    this.DataInicialEmissao = PropertyEntity({ text: "Período inicial de emissão da Fatura: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFinalEmissao = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataInicialQuitacao = PropertyEntity({ text: "Período inicial da quitação da Fatura: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFinalQuitacao = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true), val: ko.observable("") });
    this.GruposPessoas = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, url: "GrupoPessoas/ObterTodos", params: { Tipo: 0, Ativo: _statusPesquisa.Todos, OpcaoSemGrupo: true }, text: "Grupo de Pessoas: ", options: ko.observable(new Array()), issue: 58, visible: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-3 col-lg-3") });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridDescontoAcrescimoFatura.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function loadRelatorioDescontoAcrescimoFatura() {

    _pesquisaDescontoAcrescimoFatura = new PesquisaDescontoAcrescimoFatura();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridDescontoAcrescimoFatura = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/DescontoAcrescimoFatura/Pesquisa", _pesquisaDescontoAcrescimoFatura, null, null, 10);
    _gridDescontoAcrescimoFatura.SetPermitirEdicaoColunas(true);

    _relatorioDescontoAcrescimoFatura = new RelatorioGlobal("Relatorios/DescontoAcrescimoFatura/BuscarDadosRelatorio", _gridDescontoAcrescimoFatura, function () {
        _relatorioDescontoAcrescimoFatura.loadRelatorio(function () {
            KoBindings(_pesquisaDescontoAcrescimoFatura, "knockoutPesquisaDescontoAcrescimoFatura");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaDescontoAcrescimoFatura");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaDescontoAcrescimoFatura");

            new BuscarClientes(_pesquisaDescontoAcrescimoFatura.Pessoa);            
            new BuscarGruposPessoas(_pesquisaDescontoAcrescimoFatura.GrupoPessoa, null, null, null, EnumTipoGrupoPessoas.Clientes);
            new BuscarConhecimentoNotaReferencia(_pesquisaDescontoAcrescimoFatura.ConhecimentoDeTransporte, RetornoCTe);
            new BuscarFatura(_pesquisaDescontoAcrescimoFatura.Fatura, RetornoFatura);

            $("#divConteudoRelatorio").show();
        })
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaDescontoAcrescimoFatura);

}

function RetornoCTe(data) {
    _pesquisaDescontoAcrescimoFatura.ConhecimentoDeTransporte.val(data.Numero + "-" + data.Serie);
    _pesquisaDescontoAcrescimoFatura.ConhecimentoDeTransporte.codEntity(data.Codigo);
}

function RetornoFatura(data) {
    _pesquisaDescontoAcrescimoFatura.Fatura.val(data.Numero);
    _pesquisaDescontoAcrescimoFatura.Fatura.codEntity(data.Codigo);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioDescontoAcrescimoFatura.gerarRelatorio("Relatorios/DescontoAcrescimoFatura/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioDescontoAcrescimoFatura.gerarRelatorio("Relatorios/DescontoAcrescimoFatura/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
