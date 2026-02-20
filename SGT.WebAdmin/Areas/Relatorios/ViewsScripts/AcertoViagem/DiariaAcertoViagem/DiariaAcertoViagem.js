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
/// <reference path="../../../../../ViewsScripts/Consultas/Justificativa.js" />
//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioDiariaAcertoViagem, _gridDiariaAcertoViagem, _pesquisaDiariaAcertoViagem, _CRUDRelatorio, _CRUDFiltrosRelatorio;


var PesquisaDiariaAcertoViagem = function () {
    this.DataInicial = PropertyEntity({ text: "Período inicial do acerto: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Até: ", val: ko.observable(""), getType: typesKnockout.date });    
    this.Motorista = PropertyEntity({ text: "Motorista:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridDiariaAcertoViagem.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******


function loadRelatorioDiariaAcertoViagem() {

    _pesquisaDiariaAcertoViagem = new PesquisaDiariaAcertoViagem();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridDiariaAcertoViagem = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/DiariaAcertoViagem/Pesquisa", _pesquisaDiariaAcertoViagem, null, null, 10);
    _gridDiariaAcertoViagem.SetPermitirEdicaoColunas(true);

    _relatorioDiariaAcertoViagem = new RelatorioGlobal("Relatorios/DiariaAcertoViagem/BuscarDadosRelatorio", _gridDiariaAcertoViagem, function () {
        _relatorioDiariaAcertoViagem.loadRelatorio(function () {
            KoBindings(_pesquisaDiariaAcertoViagem, "knockoutPesquisaDiariaAcertoViagem");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaDiariaAcertoViagem");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaDiariaAcertoViagem");

            new BuscarMotoristas(_pesquisaDiariaAcertoViagem.Motorista);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaDiariaAcertoViagem);

}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioDiariaAcertoViagem.gerarRelatorio("Relatorios/DiariaAcertoViagem/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioDiariaAcertoViagem.gerarRelatorio("Relatorios/DiariaAcertoViagem/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
