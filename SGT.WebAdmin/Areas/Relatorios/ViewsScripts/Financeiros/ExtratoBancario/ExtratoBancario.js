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
/// <reference path="../../../../../ViewsScripts/Consultas/PlanoConta.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoPessoa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioExtratoBancario, _gridExtratoBancario, _pesquisaExtratoBancario, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaExtratoBancario = function () {        
    this.Plano = PropertyEntity({ text: "Plano de Conta:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });    
    this.DataInicial = PropertyEntity({ text: "Período inicial: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridExtratoBancario.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: guid(), visible: ko.observable(true)
    });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridExtratoBancario.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function loadRelatorioExtratoBancario() {

    _pesquisaExtratoBancario = new PesquisaExtratoBancario();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridExtratoBancario = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ExtratoBancario/Pesquisa", _pesquisaExtratoBancario, null, null, 10);
    _gridExtratoBancario.SetPermitirEdicaoColunas(true);

    _relatorioExtratoBancario = new RelatorioGlobal("Relatorios/ExtratoBancario/BuscarDadosRelatorio", _gridExtratoBancario, function () {
        _relatorioExtratoBancario.loadRelatorio(function () {
            KoBindings(_pesquisaExtratoBancario, "knockoutPesquisaExtratoBancario");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaExtratoBancario");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaExtratoBancario");

            new BuscarPlanoConta(_pesquisaExtratoBancario.Plano, "Selecione a Conta Analítica", "Contas Analíticas", RetornoPlanoConta, EnumAnaliticoSintetico.Analitico);
            
            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaExtratoBancario);

}

function RetornoPlanoConta(data) {
    _pesquisaExtratoBancario.Plano.codEntity(data.Codigo);
    _pesquisaExtratoBancario.Plano.val(data.Descricao);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioExtratoBancario.gerarRelatorio("Relatorios/ExtratoBancario/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioExtratoBancario.gerarRelatorio("Relatorios/ExtratoBancario/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}