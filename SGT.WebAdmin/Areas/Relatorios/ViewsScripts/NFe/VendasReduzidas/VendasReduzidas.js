/// <reference path="../../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../../js/Global/CRUD.js" />
/// <reference path="../../../../../js/Global/knockout-3.1.0.js" />
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
/// <reference path="../../../../../ViewsScripts/Consultas/Produto.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioVendasReduzidas, _gridVendasReduzidas, _pesquisaVendasReduzidas, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaVendasReduzidas = function () {
    this.DataInicial = PropertyEntity({ text: "*Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });

    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto: ", idBtnSearch: guid(), visible: true });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridVendasReduzidas.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******


function loadRelatorioVendasReduzidas() {

    _pesquisaVendasReduzidas = new PesquisaVendasReduzidas();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridVendasReduzidas = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/VendasReduzidas/Pesquisa", _pesquisaVendasReduzidas, null, null, 10);
    _gridVendasReduzidas.SetPermitirEdicaoColunas(true);

    _relatorioVendasReduzidas = new RelatorioGlobal("Relatorios/VendasReduzidas/BuscarDadosRelatorio", _gridVendasReduzidas, function () {
        _relatorioVendasReduzidas.loadRelatorio(function () {
            KoBindings(_pesquisaVendasReduzidas, "knockoutPesquisaVendasReduzidas");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaVendasReduzidas");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaVendasReduzidas");
            new BuscarProdutoTMS(_pesquisaVendasReduzidas.Produto);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaVendasReduzidas);
}

function GerarRelatorioPDFClick(e, sender) {
    if (!string.IsNullOrWhiteSpace(_pesquisaVendasReduzidas.DataInicial.val()))
        _relatorioVendasReduzidas.gerarRelatorio("Relatorios/VendasReduzidas/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
    else
        exibirMensagem(tipoMensagem.atencao, "Campo Obrigatório", "Por Favor, Informe a Data Inicial");
}

function GerarRelatorioExcelClick(e, sender) {
    if (!string.IsNullOrWhiteSpace(_pesquisaVendasReduzidas.DataInicial.val()))
        _relatorioVendasReduzidas.gerarRelatorio("Relatorios/VendasReduzidas/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
    else
        exibirMensagem(tipoMensagem.atencao, "Campo Obrigatório", "Por Favor, Informe a Data Inicial");
}