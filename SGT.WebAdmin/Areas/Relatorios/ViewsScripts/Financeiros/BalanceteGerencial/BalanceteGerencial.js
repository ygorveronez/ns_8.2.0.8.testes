/// <reference path="../../../../../ViewsScripts/Consultas/PlanoConta.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CentroResultado.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumAnaliticoSintetico.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioBalanceteGerencial, _gridBalanceteGerencial, _pesquisaBalanceteGerencial, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaBalanceteGerencial = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.PlanoContaSintetico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Conta Gerencial Sintética:", idBtnSearch: guid(), visible: ko.observable(true), issue: 359 });
    this.CentroResultado = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Centro de Resultado:", idBtnSearch: guid() });

    this.TipoConta = PropertyEntity({ text: "Tipo de Conta:", options: EnumAnaliticoSintetico.obterOpcoesPesquisa(), val: ko.observable(EnumAnaliticoSintetico.Todos), def: EnumAnaliticoSintetico.Todos, visible: ko.observable(true) });
    this.DataInicial = PropertyEntity({ getType: typesKnockout.date, def: Global.PrimeiraDataDoMesAtual(), val: ko.observable(Global.PrimeiraDataDoMesAtual()), text: "Data Inicial:" });
    this.DataFinal = PropertyEntity({ getType: typesKnockout.date, def: Global.DataAtual(), val: ko.observable(Global.DataAtual()), text: "Data Final:" });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridBalanceteGerencial.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaBalanceteGerencial.Visible.visibleFade()) {
                _pesquisaBalanceteGerencial.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaBalanceteGerencial.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(false)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function LoadBalanceteGerencial() {
    _pesquisaBalanceteGerencial = new PesquisaBalanceteGerencial();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridBalanceteGerencial = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/BalanceteGerencial/Pesquisa", _pesquisaBalanceteGerencial);

    _gridBalanceteGerencial.SetPermitirEdicaoColunas(true);
    _gridBalanceteGerencial.SetQuantidadeLinhasPorPagina(10);

    _relatorioBalanceteGerencial = new RelatorioGlobal("Relatorios/BalanceteGerencial/BuscarDadosRelatorio", _gridBalanceteGerencial, function () {
        _relatorioBalanceteGerencial.loadRelatorio(function () {
            KoBindings(_pesquisaBalanceteGerencial, "knockoutPesquisaBalanceteGerencial", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaBalanceteGerencial", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaBalanceteGerencial", false);

            new BuscarPlanoConta(_pesquisaBalanceteGerencial.PlanoContaSintetico, "Selecionar Conta Gerencial Sintética", "Contas Gerenciais", RetornoConsultaPlanoContaSintetico, EnumAnaliticoSintetico.Sintetico);
            new BuscarCentroResultado(_pesquisaBalanceteGerencial.CentroResultado);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaBalanceteGerencial);
}

function RetornoConsultaPlanoContaSintetico(dados) {
    _pesquisaBalanceteGerencial.PlanoContaSintetico.codEntity(dados.Codigo);
    _pesquisaBalanceteGerencial.PlanoContaSintetico.val(dados.Plano + " - " + dados.Descricao);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioBalanceteGerencial.gerarRelatorio("Relatorios/BalanceteGerencial/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioBalanceteGerencial.gerarRelatorio("Relatorios/BalanceteGerencial/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
