//*******MAPEAMENTO KNOUCKOUT*******

var _gridAliquotaICMSCTe, _pesquisaAliquotaICMSCTe, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioAliquotaICMSCTe;

var PesquisaAliquotaICMSCTe = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.EstadoEmpresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "UF da Empresa:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.EstadoOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "UF de Origem:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.EstadoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "UF de Destino:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridAliquotaICMSCTe.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaAliquotaICMSCTe.Visible.visibleFade() == true) {
                _pesquisaAliquotaICMSCTe.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaAliquotaICMSCTe.Visible.visibleFade(true);
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

function LoadAliquotaICMSCTe() {
    _pesquisaAliquotaICMSCTe = new PesquisaAliquotaICMSCTe();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridAliquotaICMSCTe = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/AliquotaICMSCTe/Pesquisa", _pesquisaAliquotaICMSCTe);

    _gridAliquotaICMSCTe.SetPermitirEdicaoColunas(true);
    _gridAliquotaICMSCTe.SetQuantidadeLinhasPorPagina(10);

    _relatorioAliquotaICMSCTe = new RelatorioGlobal("Relatorios/AliquotaICMSCTe/BuscarDadosRelatorio", _gridAliquotaICMSCTe, function () {
        _relatorioAliquotaICMSCTe.loadRelatorio(function () {
            KoBindings(_pesquisaAliquotaICMSCTe, "knockoutPesquisaAliquotaICMSCTe", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaAliquotaICMSCTe", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaAliquotaICMSCTe", false);

            BuscarEstados(_pesquisaAliquotaICMSCTe.EstadoEmpresa);
            BuscarEstados(_pesquisaAliquotaICMSCTe.EstadoOrigem);
            BuscarEstados(_pesquisaAliquotaICMSCTe.EstadoDestino);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaAliquotaICMSCTe);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioAliquotaICMSCTe.gerarRelatorio("Relatorios/AliquotaICMSCTe/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioAliquotaICMSCTe.gerarRelatorio("Relatorios/AliquotaICMSCTe/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
