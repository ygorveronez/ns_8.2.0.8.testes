/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Carga.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _gridFreteContabil, _pesquisaFreteContabil, _CRUDRelatorio, _relatorioFreteContabil, _CRUDFiltrosRelatorio;

var PesquisaFreteContabil = function () {
    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });

    this.DataEmissaoInicial = PropertyEntity({ text: "Data Emissão Inicial: ", issue: 2, getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataEmissaoFinal = PropertyEntity({ text: "Data Emissão Final: ", issue: 2, getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataEmissaoInicial.dateRangeLimit = this.DataEmissaoFinal;
    this.DataEmissaoFinal.dateRangeInit = this.DataEmissaoInicial;

    this.DataLancamentoInicial = PropertyEntity({ text: "Data Lançamento Inicial: ", issue: 2, getType: typesKnockout.date, val: ko.observable(PrimeiroDiaMes()), def: "" });
    this.DataLancamentoFinal = PropertyEntity({ text: "Data Lançamento Final: ", issue: 2, getType: typesKnockout.date, val: ko.observable(UltimoDiaMes()), def: "" });
    this.DataLancamentoInicial.dateRangeLimit = this.DataLancamentoFinal;
    this.DataLancamentoFinal.dateRangeInit = this.DataLancamentoInicial;

    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid() });

    this.CentroResultado = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Centro de Resultado:", idBtnSearch: guid() });
    this.ContaContabil = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Conta Contábil:", idBtnSearch: guid() });

    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid() });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridFreteContabil.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaFreteContabil.Visible.visibleFade() == true) {
                _pesquisaFreteContabil.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaFreteContabil.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******

function LoadRelatorioFreteContabil() {

    _pesquisaFreteContabil = new PesquisaFreteContabil();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridFreteContabil = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "/Relatorios/FreteContabil/Pesquisa", _pesquisaFreteContabil, null, null, 10);
    _gridFreteContabil.SetPermitirEdicaoColunas(true);

    _relatorioFreteContabil = new RelatorioGlobal("Relatorios/FreteContabil/BuscarDadosRelatorio", _gridFreteContabil, function () {
        _relatorioFreteContabil.loadRelatorio(function () {
            KoBindings(_pesquisaFreteContabil, "knockoutPesquisaFreteContabil");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaFreteContabil");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaFreteContabil");

            new BuscarFilial(_pesquisaFreteContabil.Filial);
            new BuscarTransportadores(_pesquisaFreteContabil.Transportador);
            new BuscarClientes(_pesquisaFreteContabil.Tomador);
            new BuscarCentroResultado(_pesquisaFreteContabil.CentroResultado);
            new BuscarPlanoConta(_pesquisaFreteContabil.ContaContabil);
            new BuscarCargas(_pesquisaFreteContabil.Carga);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaFreteContabil);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioFreteContabil.gerarRelatorio("Relatorios/FreteContabil/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioFreteContabil.gerarRelatorio("Relatorios/FreteContabil/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}




//*******METODOS*******
function PrimeiroDiaMes() {
    return "01/" + moment(new Date).format("MM/YYYY");
}

function UltimoDiaMes() {
    var date = new Date;
    var ultimoDia = new Date(date.getFullYear(), date.getMonth() + 1, 0);
    return moment(ultimoDia).format("DD/MM/YYYY");
}