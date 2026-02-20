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
/// <reference path="../../../../../ViewsScripts/Consultas/Produto.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Deposito.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Carga.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoRecebimentoMercadoria.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaTipoRecebimento = [{ text: "Todos", value: 0 },
{ text: "Mercadoria", value: EnumTipoRecebimentoMercadoria.Mercadoria },
{ text: "Volume", value: EnumTipoRecebimentoMercadoria.Volume }];

var _gridRastreabilidadeVolumes, _pesquisaRastreabilidadeVolumes, _CRUDRelatorio, _relatorioRastreabilidadeVolumes, _CRUDFiltrosRelatorio;

var PesquisaRastreabilidadeVolumes = function () {


    this.NumeroPedido = PropertyEntity({ text: "Número do pedido Embarcador: " });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.ProdutoEmbarcador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto Embarcador:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.DataPedidoInicial = PropertyEntity({ text: "Data Pedido de: ", getType: typesKnockout.date });
    this.DataPedidoFinal = PropertyEntity({ text: "Data Pedido até: ", getType: typesKnockout.date });

    this.DataRecebimentoInicial = PropertyEntity({ text: "Data Recebimento de: ", getType: typesKnockout.date });
    this.DataRecebimentoFinal = PropertyEntity({ text: "Data Recebimento até: ", getType: typesKnockout.date });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridRastreabilidadeVolumes.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******

function loadRelatorioRastreabilidadeVolumes() {

    _pesquisaRastreabilidadeVolumes = new PesquisaRastreabilidadeVolumes();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridRastreabilidadeVolumes = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "/Relatorios/RastreabilidadeVolumes/Pesquisa", _pesquisaRastreabilidadeVolumes, null, null, 10);
    _gridRastreabilidadeVolumes.SetPermitirEdicaoColunas(true);

    _relatorioRastreabilidadeVolumes = new RelatorioGlobal("Relatorios/RastreabilidadeVolumes/BuscarDadosRelatorio", _gridRastreabilidadeVolumes, function () {
        _relatorioRastreabilidadeVolumes.loadRelatorio(function () {
            KoBindings(_pesquisaRastreabilidadeVolumes, "knockoutPesquisaRastreabilidadeVolumes");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaRastreabilidadeVolumes");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaRastreabilidadeVolumes");

            new BuscarProdutos(_pesquisaRastreabilidadeVolumes.ProdutoEmbarcador);
            new BuscarCargas(_pesquisaRastreabilidadeVolumes.Carga);
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaRastreabilidadeVolumes);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioRastreabilidadeVolumes.gerarRelatorio("Relatorios/RastreabilidadeVolumes/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioRastreabilidadeVolumes.gerarRelatorio("Relatorios/RastreabilidadeVolumes/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
