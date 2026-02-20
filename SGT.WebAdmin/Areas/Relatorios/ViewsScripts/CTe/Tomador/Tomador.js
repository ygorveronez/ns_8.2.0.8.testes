//*******MAPEAMENTO KNOUCKOUT*******

var _gridTomador, _pesquisaTomador, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioTomador;

var PesquisaTomador = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:",issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas:",issue: 58, idBtnSearch: guid(),  visible: ko.observable(true) });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:",issue: 972, idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataCadastroInicial = PropertyEntity({ getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), text: "Data Inicial", def: Global.DataAtual() });
    this.DataCadastroFinal = PropertyEntity({ getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), text: "Data Final", def: Global.DataAtual() });

    this.DataCadastroInicial.dateRangeLimit = this.DataCadastroFinal;
    this.DataCadastroFinal.dateRangeInit = this.DataCadastroInicial;

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridTomador.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaTomador.Visible.visibleFade() == true) {
                _pesquisaTomador.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaTomador.Visible.visibleFade(true);
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

function LoadTomador() {
    _pesquisaTomador = new PesquisaTomador();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridTomador = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/Tomador/Pesquisa", _pesquisaTomador);

    _gridTomador.SetPermitirEdicaoColunas(true);
    _gridTomador.SetQuantidadeLinhasPorPagina(10);

    _relatorioTomador = new RelatorioGlobal("Relatorios/Tomador/BuscarDadosRelatorio", _gridTomador, function () {
        _relatorioTomador.loadRelatorio(function () {
            KoBindings(_pesquisaTomador, "knockoutPesquisaTomador", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaTomador", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaTomador", false);

            new BuscarClientes(_pesquisaTomador.Tomador);
            new BuscarGruposPessoas(_pesquisaTomador.GrupoPessoas);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaTomador);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioTomador.gerarRelatorio("Relatorios/Tomador/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioTomador.gerarRelatorio("Relatorios/Tomador/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
