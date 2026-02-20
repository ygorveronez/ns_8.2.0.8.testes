//*******MAPEAMENTO KNOUCKOUT*******

var _gridDRE, _pesquisaDRE, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioDRE;

var _tipoContaPlanoConta = [
    { value: "", text: "Todos" },
    { value: EnumAnaliticoSintetico.Analitico, text: "Analítico" },
    { value: EnumAnaliticoSintetico.Sintetico, text: "Sintético" }
];

var PesquisaDRE = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
        
    this.TipoConta = PropertyEntity({ text: "Tipo de Conta:", options: _tipoContaPlanoConta, val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true) });
    this.DataInicial = PropertyEntity({ getType: typesKnockout.date, def: Global.PrimeiraDataDoMesAnterior(), val: ko.observable(Global.PrimeiraDataDoMesAnterior()), text: "Mês Anterior:" });
    this.DataFinal = PropertyEntity({ getType: typesKnockout.date, def: Global.UltimaDataDoMesAtual(), val: ko.observable(Global.UltimaDataDoMesAtual()), text: "Mês Atual:" });
    

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridDRE.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaDRE.Visible.visibleFade() == true) {
                _pesquisaDRE.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaDRE.Visible.visibleFade(true);
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

function loadDRE() {
    _pesquisaDRE = new PesquisaDRE();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridDRE = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/DRE/Pesquisa", _pesquisaDRE);

    _gridDRE.SetPermitirEdicaoColunas(true);
    _gridDRE.SetQuantidadeLinhasPorPagina(10);

    _relatorioDRE = new RelatorioGlobal("Relatorios/DRE/BuscarDadosRelatorio", _gridDRE, function () {
        _relatorioDRE.loadRelatorio(function () {
            KoBindings(_pesquisaDRE, "knockoutPesquisaDRE", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaDRE", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaDRE", false);            

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaDRE);
}


function GerarRelatorioPDFClick(e, sender) {
    _relatorioDRE.gerarRelatorio("Relatorios/DRE/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioDRE.gerarRelatorio("Relatorios/DRE/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
