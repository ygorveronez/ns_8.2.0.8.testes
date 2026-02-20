/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoEntrega.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoOcorrencia.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Localidade.js" />

//********MAPEAMENTO KNOCKOUT********

var _relatorioPedidoRetornoOcorrencia, _gridPedidoRetornoOcorrencia, _pesquisaPedidoRetornoOcorrencia, _CRUDRelatorio, _CRUDFiltrosRelatorio;


var PesquisaPedidoRetornoOcorrencia = function () {
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.NumeroPedido = PropertyEntity({ text: "Número dos pedidos:", val: ko.observable(""), def: "" });
    this.NumeroCarga = PropertyEntity({ text: "Número da carga:", val: ko.observable(""), def: "" });
    this.DataInicial = PropertyEntity({ text: "Data Inicial (Criação):", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataFinal = PropertyEntity({ text: "Data Final (Criação):", getType: typesKnockout.date, val: ko.observable(""), def: "" });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridPedidoRetornoOcorrencia.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaPedidoRetornoOcorrencia.Visible.visibleFade() === true) {
                _pesquisaPedidoRetornoOcorrencia.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaPedidoRetornoOcorrencia.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel" });
}

//*********EVENTOS**********

function LoadPedidoRetornoOcorrencia() {
    _pesquisaPedidoRetornoOcorrencia = new PesquisaPedidoRetornoOcorrencia();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridPedidoRetornoOcorrencia = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/PedidoRetornoOcorrencia/Pesquisa", _pesquisaPedidoRetornoOcorrencia, null, null, 10);
    _gridPedidoRetornoOcorrencia.SetPermitirEdicaoColunas(true);

    _relatorioPedidoRetornoOcorrencia = new RelatorioGlobal("Relatorios/PedidoRetornoOcorrencia/BuscarDadosRelatorio", _gridPedidoRetornoOcorrencia, function () {
        _relatorioPedidoRetornoOcorrencia.loadRelatorio(function () {
            KoBindings(_pesquisaPedidoRetornoOcorrencia, "knockoutPesquisaPedidoRetornoOcorrencia", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaPedidoRetornoOcorrencia", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaPedidoRetornoOcorrencia", false);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaPedidoRetornoOcorrencia);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioPedidoRetornoOcorrencia.gerarRelatorio("Relatorios/PedidoRetornoOcorrencia/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioPedidoRetornoOcorrencia.gerarRelatorio("Relatorios/PedidoRetornoOcorrencia/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}