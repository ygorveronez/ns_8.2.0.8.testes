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

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioAbastecimentoNotaEntrada, _gridAbastecimentoNotaEntrada, _pesquisaAbastecimentoNotaEntrada, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaAbastecimentoNotaEntrada = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial Abastecimento:", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final Abastecimento:", getType: typesKnockout.date });
    this.DataInicialEmissaoNota = PropertyEntity({ text: "Data Inicial Emissão Nota:", getType: typesKnockout.date });
    this.DataFinalEmissaoNota = PropertyEntity({ text: "Data Final Emissão Nota:", getType: typesKnockout.date });

    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridAbastecimentoNotaEntrada.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******


function loadRelatorioAbastecimentoNotaEntrada() {

    _pesquisaAbastecimentoNotaEntrada = new PesquisaAbastecimentoNotaEntrada();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridAbastecimentoNotaEntrada = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/AbastecimentoNotaEntrada/Pesquisa", _pesquisaAbastecimentoNotaEntrada, null, null, 10);
    _gridAbastecimentoNotaEntrada.SetPermitirEdicaoColunas(true);

    _relatorioAbastecimentoNotaEntrada = new RelatorioGlobal("Relatorios/AbastecimentoNotaEntrada/BuscarDadosRelatorio", _gridAbastecimentoNotaEntrada, function () {
        _relatorioAbastecimentoNotaEntrada.loadRelatorio(function () {
            KoBindings(_pesquisaAbastecimentoNotaEntrada, "knockoutPesquisaAbastecimentoNotaEntrada");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaAbastecimentoNotaEntrada");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaAbastecimentoNotaEntrada");

            new BuscarVeiculos(_pesquisaAbastecimentoNotaEntrada.Veiculo);
            new BuscarClientes(_pesquisaAbastecimentoNotaEntrada.Fornecedor);
            new BuscarProdutoTMS(_pesquisaAbastecimentoNotaEntrada.Produto);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaAbastecimentoNotaEntrada);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioAbastecimentoNotaEntrada.gerarRelatorio("Relatorios/AbastecimentoNotaEntrada/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioAbastecimentoNotaEntrada.gerarRelatorio("Relatorios/AbastecimentoNotaEntrada/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}