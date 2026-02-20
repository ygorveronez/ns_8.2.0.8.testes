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

var _relatorioPosicaoContasReceber, _gridPosicaoContasReceber, _pesquisaPosicaoContasReceber, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _situacaoPosicaoContasReceber = [
    { text: "Todos", value: 0 },
    { text: "CT-e Com Fatura", value: 1 },
    { text: "CT-e Sem Fatura", value: 2 }
];

var _statusPosicaoContasReceber = [
    { text: "Todos", value: 0 },
    { text: "Em Aberto", value: 1 },
    { text: "Quitado", value: 3 }
];

var _tipoCTeVinculadoCarga = [
    { text: "Todos", value: "" },
    { text: "CT-e com Carga", value: true },
    { text: "CT-e sem Carga", value: false }
];

var PesquisaPosicaoContasReceber = function () {
    this.Situacao = PropertyEntity({ val: ko.observable(0), options: _situacaoPosicaoContasReceber, def: 0, text: "Situação CT-e: ", required: false });
    this.StatusCTe = PropertyEntity({ val: ko.observable(0), options: _statusPosicaoContasReceber, def: 0, text: "Status do CT-e: ", required: false });
    this.Status = PropertyEntity({ val: ko.observable(0), options: _statusPosicaoContasReceber, def: 0, text: "Status do Título: ", required: false });
    this.ConhecimentoDeTransporte = PropertyEntity({ text: "CT-e:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), val: ko.observable(""), });
    this.Fatura = PropertyEntity({ text: "Fatura:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), val: ko.observable(""), });
    this.Pessoa = PropertyEntity({ text: "Cliente (Tomador):", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), val: ko.observable(""), });
    this.GrupoPessoaCTe = PropertyEntity({ text: "Grupo de Pessoa diferente de (Do Tomador do CT-e):", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), val: ko.observable(""), });
    this.GrupoPessoa = PropertyEntity({ text: "Grupo de Pessoa (Da Fatura):", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), val: ko.observable(""), });
    this.CTeVinculadoACarga = PropertyEntity({ val: ko.observable(0), options: _tipoCTeVinculadoCarga, def: 0, text: "Vínculo à Carga:" });

    this.DataInicialEmissao = PropertyEntity({ text: "Período inicial de emissão do CTe: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFinalEmissao = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataInicialVencimento = PropertyEntity({ text: "Período inicial de vencimento do Título: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFinalVencimento = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });

    this.DataInicialQuitacao = PropertyEntity({ text: "Período inicial da quitação do Título: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFinalQuitacao = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataInicialMovimento = PropertyEntity({ text: "Período inicial do movimento do Título: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFinalMovimento = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });

    this.ValorCTeInicial = PropertyEntity({ text: "Valor a receber do CT-e: ", val: ko.observable("0,00"), getType: typesKnockout.decimal });
    this.ValorCTeFinal = PropertyEntity({ text: "Até: ", val: ko.observable("0,00"), getType: typesKnockout.decimal });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true), val: ko.observable("") });

    this.GruposPessoas = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, url: "GrupoPessoas/ObterTodos", params: { Tipo: 0, Ativo: _statusPesquisa.Todos, OpcaoSemGrupo: true }, text: "Grupo de Pessoas (Do Tomador do CT-e): ", options: ko.observable(new Array()), issue: 58, visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridPosicaoContasReceber.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function loadRelatorioPosicaoContasReceber() {

    _pesquisaPosicaoContasReceber = new PesquisaPosicaoContasReceber();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridPosicaoContasReceber = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/PosicaoContasReceber/Pesquisa", _pesquisaPosicaoContasReceber, null, null, 10);
    _gridPosicaoContasReceber.SetPermitirEdicaoColunas(true);

    _relatorioPosicaoContasReceber = new RelatorioGlobal("Relatorios/PosicaoContasReceber/BuscarDadosRelatorio", _gridPosicaoContasReceber, function () {
        _relatorioPosicaoContasReceber.loadRelatorio(function () {
            KoBindings(_pesquisaPosicaoContasReceber, "knockoutPesquisaPosicaoContasReceber");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaPosicaoContasReceber");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaPosicaoContasReceber");

            new BuscarClientes(_pesquisaPosicaoContasReceber.Pessoa);
            new BuscarGruposPessoas(_pesquisaPosicaoContasReceber.GrupoPessoaCTe, null, null, null, EnumTipoGrupoPessoas.Clientes);
            new BuscarGruposPessoas(_pesquisaPosicaoContasReceber.GrupoPessoa, null, null, null, EnumTipoGrupoPessoas.Clientes);
            new BuscarConhecimentoNotaReferencia(_pesquisaPosicaoContasReceber.ConhecimentoDeTransporte, RetornoCTe);
            new BuscarFatura(_pesquisaPosicaoContasReceber.Fatura, RetornoFatura);

            $("#divConteudoRelatorio").show();
        })
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaPosicaoContasReceber);

}

function RetornoCTe(data) {
    _pesquisaPosicaoContasReceber.ConhecimentoDeTransporte.val(data.Numero + "-" + data.Serie);
    _pesquisaPosicaoContasReceber.ConhecimentoDeTransporte.codEntity(data.Codigo);
}

function RetornoFatura(data) {
    _pesquisaPosicaoContasReceber.Fatura.val(data.Numero);
    _pesquisaPosicaoContasReceber.Fatura.codEntity(data.Codigo);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioPosicaoContasReceber.gerarRelatorio("Relatorios/PosicaoContasReceber/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioPosicaoContasReceber.gerarRelatorio("Relatorios/PosicaoContasReceber/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
