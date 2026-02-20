/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoLogAcesso.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridLogAcesso, _pesquisaLogAcesso, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioLogAcesso;

var PesquisaLogAcesso = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid() });

    this.DataInicial = PropertyEntity({ getType: typesKnockout.date, text: "Data Inicial:" });
    this.DataFinal = PropertyEntity({ getType: typesKnockout.date, text: "Data Final:" });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Colaborador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoLogAcesso = PropertyEntity({ val: ko.observable(EnumTipoLogAcesso.Todas), options: EnumTipoLogAcesso.obterOpcoesPesquisa(), def: EnumTipoLogAcesso.Todas, text: "Tipo de Log: ", visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridLogAcesso.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaLogAcesso.Visible.visibleFade()) {
                _pesquisaLogAcesso.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaLogAcesso.Visible.visibleFade(true);
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

function LoadRelatorioLogAcesso() {
    _pesquisaLogAcesso = new PesquisaLogAcesso();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridLogAcesso = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/LogAcesso/Pesquisa", _pesquisaLogAcesso);

    _gridLogAcesso.SetPermitirEdicaoColunas(true);
    _gridLogAcesso.SetQuantidadeLinhasPorPagina(10);

    _relatorioLogAcesso = new RelatorioGlobal("Relatorios/LogAcesso/BuscarDadosRelatorio", _gridLogAcesso, function () {
        _relatorioLogAcesso.loadRelatorio(function () {
            KoBindings(_pesquisaLogAcesso, "knockoutPesquisaLogAcesso", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaLogAcesso", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaLogAcesso", false);

            new BuscarFuncionario(_pesquisaLogAcesso.Usuario);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaLogAcesso);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioLogAcesso.gerarRelatorio("Relatorios/LogAcesso/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioLogAcesso.gerarRelatorio("Relatorios/LogAcesso/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
