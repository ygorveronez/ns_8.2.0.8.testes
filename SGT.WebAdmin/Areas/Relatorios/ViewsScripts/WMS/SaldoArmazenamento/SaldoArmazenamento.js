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
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoRecebimentoMercadoria.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaTipoRecebimento = [{ text: "Todos", value: 0 },
{ text: "Mercadoria", value: EnumTipoRecebimentoMercadoria.Mercadoria },
{ text: "Volume", value: EnumTipoRecebimentoMercadoria.Volume }];

var _gridSaldoArmazenamento, _pesquisaSaldoArmazenamento, _CRUDRelatorio, _relatorioSaldoArmazenamento, _CRUDFiltrosRelatorio;

var PesquisaSaldoArmazenamento = function () {
    this.ProdutoEmbarcador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto Embarcador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Deposito = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Depósito:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Bloco = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Bloco:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Posicao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Posição:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Rua = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Rua:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.CodigoBarras = PropertyEntity({ text: "Código de Barras: " });
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.DataVencimentoInicial = PropertyEntity({ text: "Data Vencimento de: ", getType: typesKnockout.date });
    this.DataVencimentoFinal = PropertyEntity({ text: "Até: ", getType: typesKnockout.date });
    this.SaldoDisponivel = PropertyEntity({ getType: typesKnockout.bool, text: "Gerar somente com o saldo disponível?", val: ko.observable(false), def: false });

    this.TipoRecebimento = PropertyEntity({ val: ko.observable(0), options: _pesquisaTipoRecebimento, def: 0, text: "Tipo Recebimento: " });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridSaldoArmazenamento.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******

function loadRelatorioSaldoArmazenamento() {

    _pesquisaSaldoArmazenamento = new PesquisaSaldoArmazenamento();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridSaldoArmazenamento = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "/Relatorios/SaldoArmazenamento/Pesquisa", _pesquisaSaldoArmazenamento, null, null, 10);
    _gridSaldoArmazenamento.SetPermitirEdicaoColunas(true);

    _relatorioSaldoArmazenamento = new RelatorioGlobal("Relatorios/SaldoArmazenamento/BuscarDadosRelatorio", _gridSaldoArmazenamento, function () {
        _relatorioSaldoArmazenamento.loadRelatorio(function () {
            KoBindings(_pesquisaSaldoArmazenamento, "knockoutPesquisaSaldoArmazenamento");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaSaldoArmazenamento");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaSaldoArmazenamento");

            new BuscarProdutos(_pesquisaSaldoArmazenamento.ProdutoEmbarcador);
            new BuscarDeposito(_pesquisaSaldoArmazenamento.Deposito);
            new BuscarDepositoBloco(_pesquisaSaldoArmazenamento.Bloco);
            new BuscarDepositoPosicao(_pesquisaSaldoArmazenamento.Posicao);
            new BuscarDepositoRua(_pesquisaSaldoArmazenamento.Rua);
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaSaldoArmazenamento);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioSaldoArmazenamento.gerarRelatorio("Relatorios/SaldoArmazenamento/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioSaldoArmazenamento.gerarRelatorio("Relatorios/SaldoArmazenamento/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
