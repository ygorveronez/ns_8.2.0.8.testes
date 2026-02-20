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
/// <reference path="../../../../../ViewsScripts/Consultas/Localidade.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumAnaliticoSintetico.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumDebitoCredito.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumMoedaCotacaoBancoCentral.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/PlanoConta.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoPessoa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioExtratoConta, _gridExtratoConta, _pesquisaExtratoConta, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaExtratoConta = function () {
    this.Codigo = PropertyEntity({ text: "Código: ", val: ko.observable(""), getType: typesKnockout.int, configInt: { precision: 0, allowZero: true }, def: ko.observable("") });
    this.NumeroDocumento = PropertyEntity({ text: "Número do Documento:" });

    this.DataInicial = PropertyEntity({ text: "Período inicial: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataBaseInicial = PropertyEntity({ text: "Período de base inicial: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataBaseFinal = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;
    this.DataBaseInicial.dateRangeLimit = this.DataBaseFinal;
    this.DataBaseFinal.dateRangeInit = this.DataBaseInicial;

    this.Plano = PropertyEntity({ text: "Plano de Contas Analítica:", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.CentroResultado = PropertyEntity({ text: "Centro de Resultado/Custo:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Pessoa = PropertyEntity({ text: "Pessoa Favorecida:", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.GrupoPessoa = PropertyEntity({ text: "Grupo de Pessoas Favorecida:", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.PlanoContaSintetica = PropertyEntity({ text: "Plano de Contas Sintética:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.PlanoContaContrapartida = PropertyEntity({ text: "Plano de Contas Contrapartida:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.TipoDebitoCredito = PropertyEntity({ text: "Tipo Movimentação:", options: EnumDebitoCredito.ObterOpcoesPesquisa(), val: ko.observable(EnumDebitoCredito.Todos), def: EnumDebitoCredito.Todos });
    this.MoedaCotacaoBancoCentral = PropertyEntity({ text: "Moeda:", options: EnumMoedaCotacaoBancoCentral.obterOpcoesPesquisa(), val: ko.observable(EnumMoedaCotacaoBancoCentral.Todas), def: EnumMoedaCotacaoBancoCentral.Todas, visible: ko.observable(_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridExtratoConta.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaExtratoConta.Visible.visibleFade()) {
                _pesquisaExtratoConta.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaExtratoConta.Visible.visibleFade(true);
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

function loadRelatorioExtratoConta() {

    _pesquisaExtratoConta = new PesquisaExtratoConta();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridExtratoConta = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ExtratoConta/Pesquisa", _pesquisaExtratoConta, null, null, 10);
    _gridExtratoConta.SetPermitirEdicaoColunas(true);

    _relatorioExtratoConta = new RelatorioGlobal("Relatorios/ExtratoConta/BuscarDadosRelatorio", _gridExtratoConta, function () {
        _relatorioExtratoConta.loadRelatorio(function () {
            KoBindings(_pesquisaExtratoConta, "knockoutPesquisaExtratoConta");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaExtratoConta");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaExtratoConta");

            new BuscarPlanoConta(_pesquisaExtratoConta.Plano, "Selecione a Conta Analítica", "Contas Analíticas", RetornoPlanoConta, EnumAnaliticoSintetico.Analitico);
            new BuscarPlanoConta(_pesquisaExtratoConta.PlanoContaContrapartida, "Selecione a Conta Analítica", "Contas Analíticas", RetornoPlanoContaContrapartida, EnumAnaliticoSintetico.Analitico);
            new BuscarPlanoConta(_pesquisaExtratoConta.PlanoContaSintetica, "Selecione a Conta Sintética", "Contas Sintéticas", RetornoPlanoContaSintetica, EnumAnaliticoSintetico.Sintetico);
            new BuscarCentroResultado(_pesquisaExtratoConta.CentroResultado);
            new BuscarClientes(_pesquisaExtratoConta.Pessoa);
            new BuscarGruposPessoas(_pesquisaExtratoConta.GrupoPessoa, null, null, null, EnumTipoGrupoPessoas.Clientes);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaExtratoConta);
}

function RetornoPlanoConta(data) {
    _pesquisaExtratoConta.Plano.codEntity(data.Codigo);
    _pesquisaExtratoConta.Plano.val(data.Descricao);
}

function RetornoPlanoContaContrapartida(data) {
    _pesquisaExtratoConta.PlanoContaContrapartida.codEntity(data.Codigo);
    _pesquisaExtratoConta.PlanoContaContrapartida.val(data.Descricao);
}

function RetornoPlanoContaSintetica(data) {
    _pesquisaExtratoConta.PlanoContaSintetica.codEntity(data.Codigo);
    _pesquisaExtratoConta.PlanoContaSintetica.val(data.Descricao);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioExtratoConta.gerarRelatorio("Relatorios/ExtratoConta/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioExtratoConta.gerarRelatorio("Relatorios/ExtratoConta/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}