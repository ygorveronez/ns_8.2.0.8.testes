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
/// <reference path="../../../../../ViewsScripts/Consultas/Carga.js" />
//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioCargaCompartilhada, _gridCargaCompartilhada, _pesquisaCargaCompartilhada, _CRUDRelatorio, _CRUDFiltrosRelatorio;


var PesquisaCargaCompartilhada = function () {
    this.DataInicial = PropertyEntity({ text: "Período Inicial: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Período Final: ", val: ko.observable(""), getType: typesKnockout.date });
    this.Carga = PropertyEntity({ text: "Carga:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.NumeroAcerto = PropertyEntity({ text: "N° Acerto:", val: ko.observable(0), def: 0, getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.Motorista = PropertyEntity({ text: "Motorista:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridCargaCompartilhada.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******


function loadRelatorioCargaCompartilhada() {

    _pesquisaCargaCompartilhada = new PesquisaCargaCompartilhada();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridCargaCompartilhada = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/CargaCompartilhada/Pesquisa", _pesquisaCargaCompartilhada, null, null, 10);
    _gridCargaCompartilhada.SetPermitirEdicaoColunas(true);

    _relatorioCargaCompartilhada = new RelatorioGlobal("Relatorios/CargaCompartilhada/BuscarDadosRelatorio", _gridCargaCompartilhada, function () {
        _relatorioCargaCompartilhada.loadRelatorio(function () {
            KoBindings(_pesquisaCargaCompartilhada, "knockoutPesquisaCargaCompartilhada");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaCargaCompartilhada");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaCargaCompartilhada");

            new BuscarCargas(_pesquisaCargaCompartilhada.Carga);
            new BuscarMotoristas(_pesquisaCargaCompartilhada.Motorista);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaCargaCompartilhada);

}

function RetornoBuscarAcertoViagem(data) {
    _pesquisaCargaCompartilhada.AcertoViagem.codEntity(data.Codigo);
    _pesquisaCargaCompartilhada.AcertoViagem.val(data.Numero);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioCargaCompartilhada.gerarRelatorio("Relatorios/CargaCompartilhada/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioCargaCompartilhada.gerarRelatorio("Relatorios/CargaCompartilhada/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
