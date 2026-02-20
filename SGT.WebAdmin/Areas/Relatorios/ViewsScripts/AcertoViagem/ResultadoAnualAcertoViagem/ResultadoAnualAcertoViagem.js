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
/// <reference path="../../../../../ViewsScripts/Consultas/SegmentoVeiculo.js" />
//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioResultadoAnualAcertoViagem, _gridResultadoAnualAcertoViagem, _pesquisaResultadoAnualAcertoViagem, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaResultadoAnualAcertoViagem = function () {
    this.DataInicial = PropertyEntity({ text: "Período final do acerto: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });
    this.SegmentoVeiculo = PropertyEntity({ text: "Segmento do Veículo:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridResultadoAnualAcertoViagem.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******


function loadRelatorioResultadoAnualAcertoViagem() {

    _pesquisaResultadoAnualAcertoViagem = new PesquisaResultadoAnualAcertoViagem();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridResultadoAnualAcertoViagem = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ResultadoAnualAcertoViagem/Pesquisa", _pesquisaResultadoAnualAcertoViagem, null, null, 10);
    _gridResultadoAnualAcertoViagem.SetPermitirEdicaoColunas(true);

    _relatorioResultadoAnualAcertoViagem = new RelatorioGlobal("Relatorios/ResultadoAnualAcertoViagem/BuscarDadosRelatorio", _gridResultadoAnualAcertoViagem, function () {
        _relatorioResultadoAnualAcertoViagem.loadRelatorio(function () {
            KoBindings(_pesquisaResultadoAnualAcertoViagem, "knockoutPesquisaResultadoAnualAcertoViagem");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaResultadoAnualAcertoViagem");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaResultadoAnualAcertoViagem");
            new BuscarSegmentoVeiculo(_pesquisaResultadoAnualAcertoViagem.SegmentoVeiculo);
            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaResultadoAnualAcertoViagem);

}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioResultadoAnualAcertoViagem.gerarRelatorio("Relatorios/ResultadoAnualAcertoViagem/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioResultadoAnualAcertoViagem.gerarRelatorio("Relatorios/ResultadoAnualAcertoViagem/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
