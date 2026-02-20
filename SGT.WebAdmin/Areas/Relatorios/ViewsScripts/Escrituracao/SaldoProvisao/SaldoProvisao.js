/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoGrupoPessoas.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _gridSaldoProvisao, _pesquisaSaldoProvisao, _CRUDRelatorio, _relatorioSaldoProvisao, _CRUDFiltrosRelatorio;

var PesquisaSaldoProvisao = function () {
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
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridSaldoProvisao.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaSaldoProvisao.Visible.visibleFade() == true) {
                _pesquisaSaldoProvisao.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaSaldoProvisao.Visible.visibleFade(true);
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

function LoadRelatorioSaldoProvisao() {

    _pesquisaSaldoProvisao = new PesquisaSaldoProvisao();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridSaldoProvisao = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "/Relatorios/SaldoProvisao/Pesquisa", _pesquisaSaldoProvisao, null, null, 10);
    _gridSaldoProvisao.SetPermitirEdicaoColunas(true);

    _relatorioSaldoProvisao = new RelatorioGlobal("Relatorios/SaldoProvisao/BuscarDadosRelatorio", _gridSaldoProvisao, function () {
        _relatorioSaldoProvisao.loadRelatorio(function () {
            KoBindings(_pesquisaSaldoProvisao, "knockoutPesquisaSaldoProvisao");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaSaldoProvisao");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaSaldoProvisao");

            new BuscarFilial(_pesquisaSaldoProvisao.Filial);
            new BuscarTransportadores(_pesquisaSaldoProvisao.Transportador);
            new BuscarClientes(_pesquisaSaldoProvisao.Tomador);
            new BuscarCentroResultado(_pesquisaSaldoProvisao.CentroResultado);
            new BuscarPlanoConta(_pesquisaSaldoProvisao.ContaContabil);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaSaldoProvisao);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioSaldoProvisao.gerarRelatorio("Relatorios/SaldoProvisao/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioSaldoProvisao.gerarRelatorio("Relatorios/SaldoProvisao/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
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