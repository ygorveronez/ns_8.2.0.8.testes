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
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumStatusNFe.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Produto.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/PedidoVenda.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/NotaFiscal.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioPedidosNotas, _gridPedidosNotas, _pesquisaPedidosNotas, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _statusNFe = [
    { text: "Todos", value: EnumStatusNFe.Todos },
    { text: "Em Digitação", value: EnumStatusNFe.Emitido },
    { text: "Inutilizado", value: EnumStatusNFe.Inutilizado },
    { text: "Cancelado", value: EnumStatusNFe.Cancelado },
    { text: "Autorizado", value: EnumStatusNFe.Autorizado },
    { text: "Denegado", value: EnumStatusNFe.Denegado },
    { text: "Rejeitado", value: EnumStatusNFe.Rejeitado },
    { text: "Em Processamento", value: EnumStatusNFe.EmProcessamento },
    { text: "Aguardando Assinatura do XML", value: EnumStatusNFe.AguardandoAssinar },
    { text: "Aguardando Cancelamento do XML", value: EnumStatusNFe.AguardandoCancelarAssinar },
    { text: "Aguardando Inutilizacao do XML", value: EnumStatusNFe.AguardandoInutilizarAssinar },
    { text: "Aguardando Carta Correção do XML", value: EnumStatusNFe.AguardandoCartaCorrecaoAssinar }
]

var PesquisaPedidosNotas = function () {
    this.DataNotaInicial = PropertyEntity({ text: "Data Nota Inicial: ", getType: typesKnockout.date });
    this.DataNotaFinal = PropertyEntity({ text: "Data Nota Final: ", getType: typesKnockout.date });
    this.DataPedidoInicial = PropertyEntity({ text: "Data Pedido Inicial: ", getType: typesKnockout.date });
    this.DataPedidoFinal = PropertyEntity({ text: "Data Pedido Final: ", getType: typesKnockout.date });

    this.Nota = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Nota: ", idBtnSearch: guid(), visible: true });
    this.Pedido = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pedido: ", idBtnSearch: guid(), visible: true });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa: ", idBtnSearch: guid(), visible: true });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto: ", idBtnSearch: guid(), visible: true });

    this.StatusNota = PropertyEntity({ val: ko.observable(EnumStatusNFe.Todos), options: _statusNFe, def: EnumStatusNFe.Todos, text: "Status: " });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridPedidosNotas.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******


function loadRelatorioPedidosNotas() {

    _pesquisaPedidosNotas = new PesquisaPedidosNotas();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridPedidosNotas = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/PedidoNota/Pesquisa", _pesquisaPedidosNotas, null, null, 10);
    _gridPedidosNotas.SetPermitirEdicaoColunas(true);

    _relatorioPedidosNotas = new RelatorioGlobal("Relatorios/PedidoNota/BuscarDadosRelatorio", _gridPedidosNotas, function () {
        _relatorioPedidosNotas.loadRelatorio(function () {
            KoBindings(_pesquisaPedidosNotas, "knockoutPesquisaPedidosNotas");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaPedidosNotas");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaPedidosNotas");
            new BuscarProdutoTMS(_pesquisaPedidosNotas.Produto);
            new BuscarClientes(_pesquisaPedidosNotas.Pessoa);

            new BuscarNotaFiscal(_pesquisaPedidosNotas.Nota, 1, function (data) {
                _pesquisaPedidosNotas.Nota.codEntity(data.Codigo);
                _pesquisaPedidosNotas.Nota.val(data.Numero);
            }, null);
            new BuscarPedidosVendas(_pesquisaPedidosNotas.Pedido, function (data) {
                _pesquisaPedidosNotas.Pedido.codEntity(data.Codigo);
                _pesquisaPedidosNotas.Pedido.val(data.Numero);
            }, null);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaPedidosNotas);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioPedidosNotas.gerarRelatorio("Relatorios/PedidoNota/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioPedidosNotas.gerarRelatorio("Relatorios/PedidoNota/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}