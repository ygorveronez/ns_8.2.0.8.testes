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
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioComissaoFuncionario, _gridComissaoFuncionario, _pesquisaComissaoFuncionario, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaComissaoFuncionario = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });

    this.Motorista = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridComissaoFuncionario.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******


function loadRelatorioComissaoFuncionario() {

    _pesquisaComissaoFuncionario = new PesquisaComissaoFuncionario();   
    
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridComissaoFuncionario = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ComissaoFuncionario/Pesquisa", _pesquisaComissaoFuncionario, null, null, 10);
    _gridComissaoFuncionario.SetPermitirEdicaoColunas(true);

    _relatorioComissaoFuncionario = new RelatorioGlobal("Relatorios/ComissaoFuncionario/BuscarDadosRelatorio", _gridComissaoFuncionario, function () {
        _relatorioComissaoFuncionario.loadRelatorio(function () {
            KoBindings(_pesquisaComissaoFuncionario, "knockoutComissaoFuncionario");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaComissaoFuncionario");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaComissaoFuncionario");

            new BuscarMotoristas(_pesquisaComissaoFuncionario.Motorista);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaComissaoFuncionario);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioComissaoFuncionario.gerarRelatorio("Relatorios/ComissaoFuncionario/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioComissaoFuncionario.gerarRelatorio("Relatorios/ComissaoFuncionario/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}