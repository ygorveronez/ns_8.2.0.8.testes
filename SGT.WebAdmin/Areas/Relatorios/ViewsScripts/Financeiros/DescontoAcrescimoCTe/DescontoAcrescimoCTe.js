/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Conhecimento.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Fatura.js" />
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

var _relatorioDescontoAcrescimoCTe, _gridDescontoAcrescimoCTe, _pesquisaDescontoAcrescimoCTe, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _statusDescontoAcrescimoCTe = [
    { text: "Todos", value: 0 },
    { text: "Em Aberto", value: 1 },
    { text: "Quitado", value: 3 }
];

var PesquisaDescontoAcrescimoCTe = function () {
    this.Status = PropertyEntity({ val: ko.observable(0), options: _statusDescontoAcrescimoCTe, def: 0, text: "Status do Título: ", required: false });
    this.ConhecimentoDeTransporte = PropertyEntity({ text: "CT-e:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), val: ko.observable("") });
    this.Fatura = PropertyEntity({ text: "Fatura:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), val: ko.observable("") });
    this.Pessoa = PropertyEntity({ text: "Cliente (Tomador):", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), val: ko.observable("") });
    this.GrupoPessoaCTe = PropertyEntity({ text: "Grupo de Pessoa (Do Tomador do CT-e):", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), val: ko.observable("") });
    this.GrupoPessoa = PropertyEntity({ text: "Grupo de Pessoa (Da Fatura):", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), val: ko.observable("") });

    this.DataInicialEmissao = PropertyEntity({ text: "Período inicial de emissão do CTe: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFinalEmissao = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });

    this.DataInicialPagamento = PropertyEntity({ text: "Período pagamento do Título: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFinalPagamento = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true), val: ko.observable("") });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridDescontoAcrescimoCTe.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function loadRelatorioDescontoAcrescimoCTe() {

    _pesquisaDescontoAcrescimoCTe = new PesquisaDescontoAcrescimoCTe();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridDescontoAcrescimoCTe = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/DescontoAcrescimoCTe/Pesquisa", _pesquisaDescontoAcrescimoCTe, null, null, 10);
    _gridDescontoAcrescimoCTe.SetPermitirEdicaoColunas(true);

    _relatorioDescontoAcrescimoCTe = new RelatorioGlobal("Relatorios/DescontoAcrescimoCTe/BuscarDadosRelatorio", _gridDescontoAcrescimoCTe, function () {
        _relatorioDescontoAcrescimoCTe.loadRelatorio(function () {
            KoBindings(_pesquisaDescontoAcrescimoCTe, "knockoutPesquisaDescontoAcrescimoCTe");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaDescontoAcrescimoCTe");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaDescontoAcrescimoCTe");

            new BuscarClientes(_pesquisaDescontoAcrescimoCTe.Pessoa);
            new BuscarGruposPessoas(_pesquisaDescontoAcrescimoCTe.GrupoPessoaCTe, null, null, null, EnumTipoGrupoPessoas.Clientes);
            new BuscarGruposPessoas(_pesquisaDescontoAcrescimoCTe.GrupoPessoa, null, null, null, EnumTipoGrupoPessoas.Clientes);
            new BuscarConhecimentoNotaReferencia(_pesquisaDescontoAcrescimoCTe.ConhecimentoDeTransporte, RetornoCTe);
            new BuscarFatura(_pesquisaDescontoAcrescimoCTe.Fatura, RetornoFatura);

            $("#divConteudoRelatorio").show();
        })
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaDescontoAcrescimoCTe);

}

function RetornoCTe(data) {
    _pesquisaDescontoAcrescimoCTe.ConhecimentoDeTransporte.val(data.Numero + "-" + data.Serie);
    _pesquisaDescontoAcrescimoCTe.ConhecimentoDeTransporte.codEntity(data.Codigo);
}

function RetornoFatura(data) {
    _pesquisaDescontoAcrescimoCTe.Fatura.val(data.Numero);
    _pesquisaDescontoAcrescimoCTe.Fatura.codEntity(data.Codigo);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioDescontoAcrescimoCTe.gerarRelatorio("Relatorios/DescontoAcrescimoCTe/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioDescontoAcrescimoCTe.gerarRelatorio("Relatorios/DescontoAcrescimoCTe/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
