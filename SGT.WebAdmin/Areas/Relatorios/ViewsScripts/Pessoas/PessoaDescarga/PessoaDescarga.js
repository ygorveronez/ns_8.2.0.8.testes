/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
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
/// <reference path="../../../../../js/app.config.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridPessoaDescarga, _pesquisaPessoaDescarga, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioPessoaDescarga;

var PesquisaPessoaDescarga = function () {
    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.PessoaDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa Destino:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.PessoaOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa Origem:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.ReboqueDescarga = PropertyEntity({ text: "Pessoa deixa reboque para descarga?", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridPessoaDescarga.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function LoadPessoaDescarga() {
    _pesquisaPessoaDescarga = new PesquisaPessoaDescarga();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridPessoaDescarga = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/PessoaDescarga/Pesquisa", _pesquisaPessoaDescarga);

    _gridPessoaDescarga.SetPermitirEdicaoColunas(true);
    _gridPessoaDescarga.SetQuantidadeLinhasPorPagina(15);

    _relatorioPessoaDescarga = new RelatorioGlobal("Relatorios/PessoaDescarga/BuscarDadosRelatorio", _gridPessoaDescarga, function () {
        _relatorioPessoaDescarga.loadRelatorio(function () {
            KoBindings(_pesquisaPessoaDescarga, "knockoutPesquisaPessoaDescarga", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaPessoaDescarga", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaPessoaDescarga", false);

            new BuscarClientes(_pesquisaPessoaDescarga.PessoaDestino);
            new BuscarClientes(_pesquisaPessoaDescarga.PessoaOrigem);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaPessoaDescarga);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioPessoaDescarga.gerarRelatorio("Relatorios/PessoaDescarga/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioPessoaDescarga.gerarRelatorio("Relatorios/PessoaDescarga/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}