//*******MAPEAMENTO KNOUCKOUT*******

var _gridPontuacaoComprador, _pesquisaPontuacaoComprador, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioPontuacaoComprador;


var PesquisaPontuacaoComprador = function () {    
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date });
    this.Comprador = PropertyEntity({ text: "Comprador:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid() });

    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridPontuacaoComprador.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaPontuacaoComprador.Visible.visibleFade() == true) {
                _pesquisaPontuacaoComprador.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaPontuacaoComprador.Visible.visibleFade(true);
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

function loadRelatorioPontuacaoComprador() {
    _pesquisaPontuacaoComprador = new PesquisaPontuacaoComprador();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridPontuacaoComprador = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/PontuacaoComprador/Pesquisa", _pesquisaPontuacaoComprador);

    _gridPontuacaoComprador.SetPermitirEdicaoColunas(true);
    _gridPontuacaoComprador.SetQuantidadeLinhasPorPagina(10);

    _relatorioPontuacaoComprador = new RelatorioGlobal("Relatorios/PontuacaoComprador/BuscarDadosRelatorio", _gridPontuacaoComprador, function () {
        _relatorioPontuacaoComprador.loadRelatorio(function () {
            KoBindings(_pesquisaPontuacaoComprador, "knockoutPesquisaPontuacaoComprador", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaPontuacaoComprador", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaPontuacaoComprador", false);

            // Buscas
            new BuscarFuncionario(_pesquisaPontuacaoComprador.Comprador);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaPontuacaoComprador);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioPontuacaoComprador.gerarRelatorio("Relatorios/PontuacaoComprador/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioPontuacaoComprador.gerarRelatorio("Relatorios/PontuacaoComprador/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
