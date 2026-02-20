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
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Bem.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoProdutoTMS.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Almoxarifado.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CentroResultado.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioMapaDepreciacao, _gridMapaDepreciacao, _pesquisaMapaDepreciacao, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaMapaDepreciacao = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });

    this.Bem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Patrimônio: ", idBtnSearch: guid(), visible: true });
    this.GrupoProduto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Produto: ", idBtnSearch: guid(), visible: true });
    this.Almoxarifado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Almoxarifado: ", idBtnSearch: guid(), visible: true });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Resultado: ", idBtnSearch: guid(), visible: true });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridMapaDepreciacao.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******


function loadRelatorioMapaDepreciacao() {

    _pesquisaMapaDepreciacao = new PesquisaMapaDepreciacao();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridMapaDepreciacao = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/MapaDepreciacao/Pesquisa", _pesquisaMapaDepreciacao, null, null, 10);
    _gridMapaDepreciacao.SetPermitirEdicaoColunas(true);

    _relatorioMapaDepreciacao = new RelatorioGlobal("Relatorios/MapaDepreciacao/BuscarDadosRelatorio", _gridMapaDepreciacao, function () {
        _relatorioMapaDepreciacao.loadRelatorio(function () {
            KoBindings(_pesquisaMapaDepreciacao, "knockoutPesquisaMapaDepreciacao");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaMapaDepreciacao");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaMapaDepreciacao");

            new BuscarBens(_pesquisaMapaDepreciacao.Bem);
            new BuscarGruposProdutosTMS(_pesquisaMapaDepreciacao.GrupoProduto, null);
            new BuscarAlmoxarifado(_pesquisaMapaDepreciacao.Almoxarifado);
            new BuscarCentroResultado(_pesquisaMapaDepreciacao.CentroResultado);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaMapaDepreciacao);

}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioMapaDepreciacao.gerarRelatorio("Relatorios/MapaDepreciacao/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioMapaDepreciacao.gerarRelatorio("Relatorios/MapaDepreciacao/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}