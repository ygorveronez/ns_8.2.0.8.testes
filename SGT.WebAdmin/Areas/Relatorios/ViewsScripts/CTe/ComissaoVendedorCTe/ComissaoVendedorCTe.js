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
/// <reference path="../../../../../js/app.config.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />


//********MAPEAMENTO KNOCKOUT********

var _relatorioComissaoVendedorCTe, _gridComissaoVendedorCTe, _pesquisaComissaoVendedorCTe, _CRUDRelatorio, _CRUDFiltrosRelatorio;


var PesquisaComissaoVendedorCTe = function () {
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.DataEmissaoInicial = PropertyEntity({ text: "Data de emissão inicial:", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataEmissaoFinal = PropertyEntity({ text: "Data de emssão final:", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas:", idBtnSearch: guid() });
    this.Vendedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Vendedor:", idBtnSearch: guid() });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridComissaoVendedorCTe.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel" });
}

//*********EVENTOS**********

function LoadComissaoVendedorCTe() {
    _pesquisaComissaoVendedorCTe = new PesquisaComissaoVendedorCTe();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridComissaoVendedorCTe = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ComissaoVendedorCTe/Pesquisa", _pesquisaComissaoVendedorCTe, null, null, 10);
    _gridComissaoVendedorCTe.SetPermitirEdicaoColunas(true);

    _relatorioComissaoVendedorCTe = new RelatorioGlobal("Relatorios/ComissaoVendedorCTe/BuscarDadosRelatorio", _gridComissaoVendedorCTe, function () {
        _relatorioComissaoVendedorCTe.loadRelatorio(function () {
            KoBindings(_pesquisaComissaoVendedorCTe, "knockoutPesquisaComissaoVendedorCTe", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaComissaoVendedorCTe", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaComissaoVendedorCTe", false);

            new BuscarFuncionario(_pesquisaComissaoVendedorCTe.Vendedor);
            new BuscarGruposPessoas(_pesquisaComissaoVendedorCTe.GrupoPessoas);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaComissaoVendedorCTe);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioComissaoVendedorCTe.gerarRelatorio("Relatorios/ComissaoVendedorCTe/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioComissaoVendedorCTe.gerarRelatorio("Relatorios/ComissaoVendedorCTe/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}