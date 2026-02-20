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
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioResponsavelVeiculo, _gridResponsavelVeiculo, _pesquisaResponsavelVeiculo, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaResponsavelVeiculo = function () {
    this.Veiculo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });
    this.FuncionarioResponsavel = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Funcionário Responsável:", idBtnSearch: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridResponsavelVeiculo.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******


function loadRelatorioResponsavelVeiculo() {

    _pesquisaResponsavelVeiculo = new PesquisaResponsavelVeiculo();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridResponsavelVeiculo = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ResponsavelVeiculo/Pesquisa", _pesquisaResponsavelVeiculo, null, null, 10);
    _gridResponsavelVeiculo.SetPermitirEdicaoColunas(true);

    _relatorioResponsavelVeiculo = new RelatorioGlobal("Relatorios/ResponsavelVeiculo/BuscarDadosRelatorio", _gridResponsavelVeiculo, function () {
        _relatorioResponsavelVeiculo.loadRelatorio(function () {
            KoBindings(_pesquisaResponsavelVeiculo, "knockoutPesquisaResponsavelVeiculo");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaResponsavelVeiculo");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaResponsavelVeiculo");

            new BuscarVeiculos(_pesquisaResponsavelVeiculo.Veiculo);
            new BuscarFuncionario(_pesquisaResponsavelVeiculo.FuncionarioResponsavel);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaResponsavelVeiculo);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioResponsavelVeiculo.gerarRelatorio("Relatorios/ResponsavelVeiculo/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioResponsavelVeiculo.gerarRelatorio("Relatorios/ResponsavelVeiculo/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}