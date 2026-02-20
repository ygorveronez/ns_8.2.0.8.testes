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
/// <reference path="../../../../../ViewsScripts/Consultas/Localidade.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridFilial, _pesquisaFilial, _CRUDRelatorio, _relatorioFilial, _CRUDFiltrosRelatorio;

var PesquisaFilial = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridFilial.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******

function loadRelatorioFilial() {

    _pesquisaFilial = new PesquisaFilial();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridFilial = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "/Relatorios/Filial/Pesquisa", _pesquisaFilial);
    _gridFilial.SetPermitirEdicaoColunas(true);

    _relatorioFilial = new RelatorioGlobal("Relatorios/Filial/BuscarDadosRelatorio", _gridFilial, function () {
        _relatorioFilial.loadRelatorio(function () {
            KoBindings(_pesquisaFilial, "knockoutPesquisaFilial");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaFilial");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaFiliais");
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaFilial);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioFilial.gerarRelatorio("Relatorios/Filial/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioFilial.gerarRelatorio("Relatorios/Filial/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
