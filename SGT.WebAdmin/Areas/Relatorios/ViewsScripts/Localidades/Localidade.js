/// <reference path="../../../../ViewsScripts/Consultas/Regiao.js" />
/// <reference path="../../../../ViewsScripts/Consultas/Estado.js" />
/// <reference path="../../../../ViewsScripts/Consultas/Pais.js" />
/// <reference path="../Relatorios/Global/Relatorio.js" />


//#region Mapeamento Knockout

var _pesquisaLocalidade;
var _gridLocalidade;
var _relatorioLocalidade;

var PesquisaLocalidade = function () {
    this.Estado = PropertyEntity({ text: "Estado:", codEntity: ko.observable(0), type: types.multiplesEntities, idBtnSearch: guid() });   
    this.Pais = PropertyEntity({ text: "País:", codEntity: ko.observable(0), type: types.multiplesEntities, idBtnSearch: guid() });
    this.Regiao = PropertyEntity({ text: "Região:", codEntity: ko.observable(0), type: types.multiplesEntities, idBtnSearch: guid() });
    this.Descricao = PropertyEntity({ text: "Descrição:", val: ko.observable(""), getType: typesKnockout.string });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridLocalidade.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaLocalidade.Visible.visibleFade() == true) {
                _pesquisaLocalidade.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaLocalidade.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

function loadLocalidade() {
    _pesquisaLocalidade = new PesquisaLocalidade();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridLocalidade = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/Localidade/Pesquisa", _pesquisaLocalidade, null, null, 10, null, null, null, null, 20);
    _gridLocalidade.SetPermitirEdicaoColunas(true);

    _relatorioLocalidade = new RelatorioGlobal("Relatorios/Localidade/BuscarDadosRelatorio", _gridLocalidade, function () {
        _relatorioLocalidade.loadRelatorio(function () {
            KoBindings(_pesquisaLocalidade, "knockoutPesquisaLocalidade");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaLocalidade");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaLocalidade");

            new BuscarEstados(_pesquisaLocalidade.Estado);
            new BuscarPaises(_pesquisaLocalidade.Pais);
            new BuscarRegioes(_pesquisaLocalidade.Regiao);
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaLocalidade);
}

//#endregion

//#region Métodos

function GerarRelatorioPDFClick(e, sender) {
    _relatorioLocalidade.gerarRelatorio("Relatorios/Localidade/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioLocalidade.gerarRelatorio("Relatorios/Localidade/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}

//#endregion