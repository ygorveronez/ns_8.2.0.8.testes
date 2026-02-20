
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
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../js/app.config.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridConfiguracaoTabelaFrete, _pesquisaConfiguracaoTabelaFrete, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioConfiguracaoTabelaFrete;

var _opcoesSituacaoTabelaFrete = [
    { text: "Todos", value: "" },
    { text: "Ativo", value: true },
    { text: "Inativo", value: false }
];

var _opcoesTabelaVigente = [
    { text: "Todos", value: "" },
    { text: "Apenas tabelas vigentes", value: true },
    { text: "Apenas tabelas que não estão vigentes", value: false }
];

var PesquisaConfiguracaoTabelaFrete = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.GrupoPessoas = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Grupo de Pessoas:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });

    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Situacao = PropertyEntity({ val: ko.observable(""), options: _opcoesSituacaoTabelaFrete, def: "", text: "Situação:", visible: ko.observable(true) });
    this.TabelasVigentes = PropertyEntity({ val: ko.observable(""), options: _opcoesTabelaVigente, def: "", text: "Vigência:", visible: ko.observable(true) });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridConfiguracaoTabelaFrete.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaConfiguracaoTabelaFrete.Visible.visibleFade() == true) {
                _pesquisaConfiguracaoTabelaFrete.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaConfiguracaoTabelaFrete.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function LoadConfiguracaoTabelaFrete() {
    _pesquisaConfiguracaoTabelaFrete = new PesquisaConfiguracaoTabelaFrete();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridConfiguracaoTabelaFrete = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ConfiguracaoTabelaFrete/Pesquisa", _pesquisaConfiguracaoTabelaFrete);

    _gridConfiguracaoTabelaFrete.SetPermitirEdicaoColunas(true);
    _gridConfiguracaoTabelaFrete.SetQuantidadeLinhasPorPagina(10);

    _relatorioConfiguracaoTabelaFrete = new RelatorioGlobal("Relatorios/ConfiguracaoTabelaFrete/BuscarDadosRelatorio", _gridConfiguracaoTabelaFrete, function () {
        _relatorioConfiguracaoTabelaFrete.loadRelatorio(function () {
            KoBindings(_pesquisaConfiguracaoTabelaFrete, "knockoutPesquisaConfiguracaoTabelaFrete", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaConfiguracaoTabelaFrete", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaConfiguracaoTabelaFrete", false);

            BuscarGruposPessoas(_pesquisaConfiguracaoTabelaFrete.GrupoPessoas);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaConfiguracaoTabelaFrete);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioConfiguracaoTabelaFrete.gerarRelatorio("Relatorios/ConfiguracaoTabelaFrete/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioConfiguracaoTabelaFrete.gerarRelatorio("Relatorios/ConfiguracaoTabelaFrete/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
