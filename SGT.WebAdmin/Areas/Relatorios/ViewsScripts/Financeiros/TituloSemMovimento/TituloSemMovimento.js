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
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioTituloSemMovimento, _gridTituloSemMovimento, _pesquisaTituloSemMovimento, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _tipoTitulo = [
    { text: "A Receber", value: EnumTipoTitulo.AReceber },
    { text: "A Pagar", value: EnumTipoTitulo.APagar }
];

var _statusTitulo = [
    { text: "Em Aberto", value: EnumSituacaoTitulo.EmAberto },
    { text: "Quitado", value: EnumSituacaoTitulo.Quitado },
    { text: "Cancelado", value: EnumSituacaoTitulo.Cancelado }
];

var PesquisaTituloSemMovimento = function () {
    this.TipoTitulo = PropertyEntity({ val: ko.observable(1), options: _tipoTitulo, def: 1, text: "Tipo Título:" });

    this.StatusTitulo = PropertyEntity({ val: ko.observable([EnumSituacaoTitulo.EmAberto, EnumSituacaoTitulo.Quitado]), def: [EnumSituacaoTitulo.EmAberto, EnumSituacaoTitulo.Quitado], getType: typesKnockout.selectMultiple, text: "Situação: ", options: _statusTitulo, visible: ko.observable(true) });

    this.Pessoa = PropertyEntity({ text: "Cliente / Fornecedor:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), val: ko.observable("") });

    this.DataInicialEmissao = PropertyEntity({ text: "Período inicial de emissão: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFinalEmissao = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataInicialVencimento = PropertyEntity({ text: "Período inicial de vencimento: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFinalVencimento = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true), val: ko.observable("") });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridTituloSemMovimento.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******

function loadRelatorioTituloSemMovimento() {

    _pesquisaTituloSemMovimento = new PesquisaTituloSemMovimento();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridTituloSemMovimento = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/TituloSemMovimento/Pesquisa", _pesquisaTituloSemMovimento, null, null, 10);
    _gridTituloSemMovimento.SetPermitirEdicaoColunas(true);

    _relatorioTituloSemMovimento = new RelatorioGlobal("Relatorios/TituloSemMovimento/BuscarDadosRelatorio", _gridTituloSemMovimento, function () {
        _relatorioTituloSemMovimento.loadRelatorio(function () {
            KoBindings(_pesquisaTituloSemMovimento, "knockoutPesquisaTituloSemMovimento");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaTituloSemMovimento");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaTituloSemMovimento");

            new BuscarClientes(_pesquisaTituloSemMovimento.Pessoa);

            $("#divConteudoRelatorio").show();
        })
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaTituloSemMovimento);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioTituloSemMovimento.gerarRelatorio("Relatorios/TituloSemMovimento/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioTituloSemMovimento.gerarRelatorio("Relatorios/TituloSemMovimento/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
