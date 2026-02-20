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
//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioCTEEmitido, _gridCTeEmitido, _pesquisaCTeEmitido, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaCTeEmitido = function () {
    this.DataEmissaoInicial = PropertyEntity({ text: "Data Emissão Inicial: ",issue: 2,  val: ko.observable(""), getType: typesKnockout.date });
    this.DataEmissaoFinal = PropertyEntity({ text: "Data Emissão Final: ", issue: 2, val: ko.observable(""), getType: typesKnockout.date });
    this.DataAutorizacaoInicial = PropertyEntity({ text: "Data Autorização Inicial: ", issue: 2, val: ko.observable(""), getType: typesKnockout.date });
    this.DataAutorizacaoFinal = PropertyEntity({ text: "Data Autorização Final: ", issue: 2, val: ko.observable(""), getType: typesKnockout.date });
    this.Transportador = PropertyEntity({ text: "Transportador:",issue: 69, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Embarcador = PropertyEntity({ text: "Embarcador:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Filial = PropertyEntity({ text: "Filial:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:",issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridCTeEmitido.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******

function loadRelatorioCTeEmitido() {

    _pesquisaCTeEmitido = new PesquisaCTeEmitido();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridCTeEmitido = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/CTeEmitido/Pesquisa", _pesquisaCTeEmitido, null, null, 10);
    _gridCTeEmitido.SetPermitirEdicaoColunas(true);

    _relatorioCTEEmitido = new RelatorioGlobal("Relatorios/CTeEmitido/BuscarDadosRelatorio", _gridCTeEmitido, function () {
        _relatorioCTEEmitido.loadRelatorio(function () {
            KoBindings(_pesquisaCTeEmitido, "knockoutPesquisaCTeEmitido");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaCTeEmitido");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaCTes");

            new BuscarFilial(_pesquisaCTeEmitido.Filial);
            new BuscarTransportadores(_pesquisaCTeEmitido.Transportador);
            new BuscarClientes(_pesquisaCTeEmitido.Embarcador);

            $("#divConteudoRelatorio").show();
        })
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaCTeEmitido);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        _pesquisaCTeEmitido.Filial.visible(false);
    }
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioCTEEmitido.gerarRelatorio("Relatorios/CTeEmitido/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioCTEEmitido.gerarRelatorio("Relatorios/CTeEmitido/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}