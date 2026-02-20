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
/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoLancamentoColaborador.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoColaborador.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioColaboradorSituacaoLancamento, _gridColaboradorSituacaoLancamento, _pesquisaColaboradorSituacaoLancamento, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaColaboradorSituacaoLancamento = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date });

    this.Colaborador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Colaborador: ", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable([]), def: [], getType: typesKnockout.selectMultiple, options: EnumSituacaoLancamentoColaborador.obterOpcoes(), text: "Situação:", visible: ko.observable(true) });
    this.SituacaoColaborador = PropertyEntity({ val: ko.observable([]), def: [], getType: typesKnockout.selectMultiple, options: EnumSituacaoColaborador.obterOpcoes(), text: "Colaborador Situação:", visible: ko.observable(true) });
    this.Frota = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Frota / Veículo: ", idBtnSearch: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridColaboradorSituacaoLancamento.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******


function loadRelatorioColaboradorSituacaoLancamento() {

    _pesquisaColaboradorSituacaoLancamento = new PesquisaColaboradorSituacaoLancamento();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridColaboradorSituacaoLancamento = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ColaboradorSituacaoLancamento/Pesquisa", _pesquisaColaboradorSituacaoLancamento, null, null, 10);
    _gridColaboradorSituacaoLancamento.SetPermitirEdicaoColunas(true);

    _relatorioColaboradorSituacaoLancamento = new RelatorioGlobal("Relatorios/ColaboradorSituacaoLancamento/BuscarDadosRelatorio", _gridColaboradorSituacaoLancamento, function () {
        _relatorioColaboradorSituacaoLancamento.loadRelatorio(function () {
            KoBindings(_pesquisaColaboradorSituacaoLancamento, "knockoutPesquisaColaboradorSituacaoLancamento");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaColaboradorSituacaoLancamento");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaColaboradorSituacaoLancamento");

            new BuscarFuncionario(_pesquisaColaboradorSituacaoLancamento.Colaborador);
            new BuscarVeiculos(_pesquisaColaboradorSituacaoLancamento.Frota);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaColaboradorSituacaoLancamento);

}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioColaboradorSituacaoLancamento.gerarRelatorio("Relatorios/ColaboradorSituacaoLancamento/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioColaboradorSituacaoLancamento.gerarRelatorio("Relatorios/ColaboradorSituacaoLancamento/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}