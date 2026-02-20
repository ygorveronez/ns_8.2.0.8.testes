/// <reference path="../../../../../ViewsScripts/Consultas/AcertoViagem.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoGrupoPessoas.js" />
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
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoPessoa.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Produto.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/SegmentoVeiculo.js" />
//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioUltimoAcertoMotorista, _gridUltimoAcertoMotorista, _pesquisaUltimoAcertoMotorista, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _tipoMotorista = [
    { text: "Todos", value: 0 },
    { text: "Proprio", value: 1 },
    { text: "Terceiro", value: 2 }
];

var PesquisaUltimoAcertoMotorista = function () {    
    this.StatusMotorista = PropertyEntity({ val: ko.observable(""), options: _statusPesquisa, def: "", text: "Status Motorista: " });
    this.TipoMotorista = PropertyEntity({ val: ko.observable(_tipoMotorista.Todos), options: _tipoMotorista, def: _tipoMotorista.Todos, text: "Tipo do Motorista: " });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridUltimoAcertoMotorista.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******


function loadRelatorioUltimoAcertoMotorista() {

    _pesquisaUltimoAcertoMotorista = new PesquisaUltimoAcertoMotorista();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridUltimoAcertoMotorista = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/UltimoAcertoMotorista/Pesquisa", _pesquisaUltimoAcertoMotorista, null, null, 10);
    _gridUltimoAcertoMotorista.SetPermitirEdicaoColunas(true);

    _relatorioUltimoAcertoMotorista = new RelatorioGlobal("Relatorios/UltimoAcertoMotorista/BuscarDadosRelatorio", _gridUltimoAcertoMotorista, function () {
        _relatorioUltimoAcertoMotorista.loadRelatorio(function () {
            KoBindings(_pesquisaUltimoAcertoMotorista, "knockoutPesquisaUltimoAcertoMotorista");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaUltimoAcertoMotorista");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaUltimoAcertoMotorista");

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaUltimoAcertoMotorista);

}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioUltimoAcertoMotorista.gerarRelatorio("Relatorios/UltimoAcertoMotorista/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioUltimoAcertoMotorista.gerarRelatorio("Relatorios/UltimoAcertoMotorista/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
