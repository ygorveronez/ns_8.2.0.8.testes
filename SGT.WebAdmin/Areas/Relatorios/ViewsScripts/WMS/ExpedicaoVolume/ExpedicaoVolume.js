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
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Carga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridExpedicaoVolume, _pesquisaExpedicaoVolume, _CRUDRelatorio, _relatorioExpedicaoVolume, _CRUDFiltrosRelatorio;

var PesquisaExpedicaoVolume = function () {
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.DataExpedicaoInicial = PropertyEntity({ text: "Data Expedição de: ", getType: typesKnockout.date });
    this.DataExpedicaoFinal = PropertyEntity({ text: "Até: ", getType: typesKnockout.date });
    this.DataEmbarqueInicial = PropertyEntity({ text: "Data Embarque de: ", getType: typesKnockout.date });
    this.DataEmbarqueFinal = PropertyEntity({ text: "Até: ", getType: typesKnockout.date });
    this.Conferente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Conferente:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.NumeroPedido = PropertyEntity({ text: "Número Pedido: " });
    this.NumeroNota = PropertyEntity({ text: "Número Nota: " });
    this.CodigoBarras = PropertyEntity({ text: "Cód. Barras: " });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridExpedicaoVolume.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******

function loadRelatorioExpedicaoVolume() {

    _pesquisaExpedicaoVolume = new PesquisaExpedicaoVolume();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridExpedicaoVolume = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "/Relatorios/ExpedicaoVolume/Pesquisa", _pesquisaExpedicaoVolume, null, null, 10);
    _gridExpedicaoVolume.SetPermitirEdicaoColunas(true);

    _relatorioExpedicaoVolume = new RelatorioGlobal("Relatorios/ExpedicaoVolume/BuscarDadosRelatorio", _gridExpedicaoVolume, function () {
        _relatorioExpedicaoVolume.loadRelatorio(function () {
            KoBindings(_pesquisaExpedicaoVolume, "knockoutPesquisaExpedicaoVolume");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaExpedicaoVolume");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaExpedicaoVolume");

            new BuscarCargas(_pesquisaExpedicaoVolume.Carga);
            new BuscarFuncionario(_pesquisaExpedicaoVolume.Conferente);
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaExpedicaoVolume);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioExpedicaoVolume.gerarRelatorio("Relatorios/ExpedicaoVolume/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioExpedicaoVolume.gerarRelatorio("Relatorios/ExpedicaoVolume/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
