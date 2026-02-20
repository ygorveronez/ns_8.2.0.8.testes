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
/// <reference path="../../../../../js/app.config.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CentroResultado.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/SegmentoVeiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoDespesaFinanceira.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoDespesaFinanceira.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumOrigemRateioDespesaVeiculo.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridRateioDespesaVeiculo, _pesquisaRateioDespesaVeiculo, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioRateioDespesaVeiculo;

var PesquisaRateioDespesaVeiculo = function () {
    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.GrupoDespesa = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(""), defCodEntity: "", text: "Grupo de Despesa:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(""), defCodEntity: "", text: "Veículo:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.CentroResultado = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(""), defCodEntity: "", text: "Centro de Resultado:", idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.UtilizarCentroResultadoNoRateioDespesaVeiculo) });
    this.Segmentos = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(""), defCodEntity: "", text: "Segmentos:", idBtnSearch: guid(), visible: ko.observable(!_CONFIGURACAO_TMS.UtilizarCentroResultadoNoRateioDespesaVeiculo) });
    this.TipoDespesa = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(""), defCodEntity: "", text: "Tipo de Despesa:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), defCodEntity: "", text: "Pessoa: ", idBtnSearch: guid(), visible: ko.observable(true) });

    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.DataLancamentoInicial = PropertyEntity({ text: "Data Lançamento Inicial: ", getType: typesKnockout.date });
    this.DataLancamentoFinal = PropertyEntity({ text: "Data Lançamento Final: ", getType: typesKnockout.date });
    this.DataLancamentoInicial.dateRangeLimit = this.DataLancamentoFinal;
    this.DataLancamentoFinal.dateRangeInit = this.DataLancamentoInicial;

    this.OrigemRateio = PropertyEntity({ text: "Origem Rateio:", val: ko.observable(EnumOrigemRateioDespesaVeiculo.Todos), options: EnumOrigemRateioDespesaVeiculo.obterOpcoesPesquisa(), def: EnumOrigemRateioDespesaVeiculo.Todos });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridRateioDespesaVeiculo.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaRateioDespesaVeiculo.Visible.visibleFade()) {
                _pesquisaRateioDespesaVeiculo.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaRateioDespesaVeiculo.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function LoadRateioDespesaVeiculo() {
    _pesquisaRateioDespesaVeiculo = new PesquisaRateioDespesaVeiculo();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridRateioDespesaVeiculo = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/RateioDespesaVeiculo/Pesquisa", _pesquisaRateioDespesaVeiculo);

    _gridRateioDespesaVeiculo.SetPermitirEdicaoColunas(true);
    _gridRateioDespesaVeiculo.SetQuantidadeLinhasPorPagina(10);

    _relatorioRateioDespesaVeiculo = new RelatorioGlobal("Relatorios/RateioDespesaVeiculo/BuscarDadosRelatorio", _gridRateioDespesaVeiculo, function () {
        _relatorioRateioDespesaVeiculo.loadRelatorio(function () {
            KoBindings(_pesquisaRateioDespesaVeiculo, "knockoutPesquisaRateioDespesaVeiculo", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaRateioDespesaVeiculo", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaRateioDespesaVeiculo", false);

            new BuscarGrupoDespesaFinanceira(_pesquisaRateioDespesaVeiculo.GrupoDespesa);
            new BuscarVeiculos(_pesquisaRateioDespesaVeiculo.Veiculo);
            new BuscarCentroResultado(_pesquisaRateioDespesaVeiculo.CentroResultado);
            new BuscarSegmentoVeiculo(_pesquisaRateioDespesaVeiculo.Segmentos);
            new BuscarTipoDespesaFinanceira(_pesquisaRateioDespesaVeiculo.TipoDespesa);
            new BuscarClientes(_pesquisaRateioDespesaVeiculo.Pessoa);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaRateioDespesaVeiculo);

}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioRateioDespesaVeiculo.gerarRelatorio("Relatorios/RateioDespesaVeiculo/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioRateioDespesaVeiculo.gerarRelatorio("Relatorios/RateioDespesaVeiculo/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}