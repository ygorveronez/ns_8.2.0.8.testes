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
/// <reference path="../../../../../ViewsScripts/Consultas/Empresa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioVencimentoCertificados, _gridVencimentoCertificados, _pesquisaVencimentoCertificados, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaVencimentoCertificados = function () {
    this.DataInicial = PropertyEntity({ text: Localization.Resources.Transportadores.Transportadores.DataVencimentoInicial.getFieldDescription(), getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: Localization.Resources.Transportadores.Transportadores.DataVencimentoFinal.getFieldDescription(), getType: typesKnockout.date });

    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Transportadores.Transportadores.Empresa.getFieldDescription(), idBtnSearch: guid(), visible: true });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Transportadores.Transportadores.TipoRelatorio.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridVencimentoCertificados.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Transportadores.Transportadores.Preview, idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: Localization.Resources.Transportadores.Transportadores.GerarPDF });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: Localization.Resources.Transportadores.Transportadores.GerarPlanilhaExcel, idGrid: guid() });
}

//*******EVENTOS*******


function loadRelatorioVencimentoCertificados() {

    _pesquisaVencimentoCertificados = new PesquisaVencimentoCertificados();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridVencimentoCertificados = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/VencimentoCertificado/Pesquisa", _pesquisaVencimentoCertificados, null, null, 10);
    _gridVencimentoCertificados.SetPermitirEdicaoColunas(true);

    _relatorioVencimentoCertificados = new RelatorioGlobal("Relatorios/VencimentoCertificado/BuscarDadosRelatorio", _gridVencimentoCertificados, function () {
        _relatorioVencimentoCertificados.loadRelatorio(function () {
            KoBindings(_pesquisaVencimentoCertificados, "knockoutPesquisaVencimentoCertificados");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaVencimentoCertificados");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaVencimentoCertificados");
            new BuscarEmpresa(_pesquisaVencimentoCertificados.Empresa);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaVencimentoCertificados);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioVencimentoCertificados.gerarRelatorio("Relatorios/VencimentoCertificado/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioVencimentoCertificados.gerarRelatorio("Relatorios/VencimentoCertificado/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}