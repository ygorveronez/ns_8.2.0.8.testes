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
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumFinalidadeTipoMovimento.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/PlanoConta.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoPessoa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioConciliacaoBancaria, _gridConciliacaoBancaria, _pesquisaConciliacaoBancaria, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaConciliacaoBancaria = function () {
    this.Plano = PropertyEntity({ text: "Plano de Conta:", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.DataInicial = PropertyEntity({ text: "Período inicial: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.DebitoCredito = PropertyEntity({ val: ko.observable(EnumDebitoCredito.Todos), options: EnumDebitoCredito.ObterOpcoesPesquisa(), text: "Débito/Crédito: ", def: EnumDebitoCredito.Todos });
    this.Observacao = PropertyEntity({ text: "Observação:", val: ko.observable(""), def: "" });
    this.TipoMovimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo movimento:", idBtnSearch: guid() });
    this.NumeroDocumento = PropertyEntity({ text: "Nº Doc:", val: ko.observable(""), def: "", maxlength: 20 });
    this.CodigoMovimento = PropertyEntity({ getType: typesKnockout.int, text: "Código Movimento:", val: ko.observable(0), def: 0, configInt: { precision: 0, allowZero: false, thousands: "" } });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridConciliacaoBancaria.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: guid(), visible: ko.observable(true)
    });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridConciliacaoBancaria.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function loadRelatorioConciliacaoBancaria() {

    _pesquisaConciliacaoBancaria = new PesquisaConciliacaoBancaria();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridConciliacaoBancaria = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ConciliacaoBancaria/Pesquisa", _pesquisaConciliacaoBancaria, null, null, 10);
    _gridConciliacaoBancaria.SetPermitirEdicaoColunas(true);

    _relatorioConciliacaoBancaria = new RelatorioGlobal("Relatorios/ConciliacaoBancaria/BuscarDadosRelatorio", _gridConciliacaoBancaria, function () {
        _relatorioConciliacaoBancaria.loadRelatorio(function () {
            KoBindings(_pesquisaConciliacaoBancaria, "knockoutPesquisaConciliacaoBancaria");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaConciliacaoBancaria");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaConciliacaoBancaria");

            new BuscarPlanoConta(_pesquisaConciliacaoBancaria.Plano, "Selecione a Conta Analítica", "Contas Analíticas", RetornoPlanoConta, EnumAnaliticoSintetico.Analitico, null, null, null, null, null);
            new BuscarTipoMovimento(_pesquisaConciliacaoBancaria.TipoMovimento, null, null, null, null, EnumFinalidadeTipoMovimento.MovimentoFinanceiro);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaConciliacaoBancaria);

}

function RetornoPlanoConta(data) {
    _pesquisaConciliacaoBancaria.Plano.codEntity(data.Codigo);
    _pesquisaConciliacaoBancaria.Plano.val(data.Descricao);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioConciliacaoBancaria.gerarRelatorio("Relatorios/ConciliacaoBancaria/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioConciliacaoBancaria.gerarRelatorio("Relatorios/ConciliacaoBancaria/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}