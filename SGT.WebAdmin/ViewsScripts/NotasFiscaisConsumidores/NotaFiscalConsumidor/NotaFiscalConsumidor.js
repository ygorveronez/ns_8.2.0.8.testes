/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Consultas/Servico.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumTipoArredondamento.js" />
/// <reference path="../../Enumeradores/EnumFormaTitulo.js" />
/// <reference path="../../Enumeradores/EnumStatusNFe.js" />
/// <reference path="../../Enumeradores/EnumFormaPagamentoNFCe.js" />
/// <reference path="../../Consultas/PedidoVenda.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridNotaFiscalConsumidor;
var _notaFiscalConsumidor;
var _justificativaCancelamento;
var _pesquisaNotaFiscalConsumidor;
var _gridItensNotaFiscalConsumidor;
var _casasQuantidadeProduto;
var _casasValorProduto;
var _gridParcelasNotaFiscalConsumidor;
var _detalheParcelaNotaFiscalConsumidor;
var _emitirVendaPrazoNFCe;
var _gridPedidos;

var _tipoItem = [
    { value: 1, text: "Produto" },
    { value: 2, text: "Serviço" }
];

var _statusNFe = [
    { text: "Todos", value: 0 },
    { text: "Em Digitação", value: EnumStatusNFe.Emitido },
    { text: "Inutilizado", value: EnumStatusNFe.Inutilizado },
    { text: "Cancelado", value: EnumStatusNFe.Cancelado },
    { text: "Autorizado", value: EnumStatusNFe.Autorizado },
    { text: "Denegado", value: EnumStatusNFe.Denegado },
    { text: "Rejeitado", value: EnumStatusNFe.Rejeitado },
    { text: "Em Processamento", value: EnumStatusNFe.EmProcessamento },
    { text: "Aguardando Assinatura", value: EnumStatusNFe.AguardandoAssinar },
    { text: "Aguardando Cancelamento do XML", value: EnumStatusNFe.AguardandoCancelarAssinar },
    { text: "Aguardando Inutilizacao do XML", value: EnumStatusNFe.AguardandoInutilizarAssinar },
    { text: "Aguardando Carta Correção do XML", value: EnumStatusNFe.AguardandoCartaCorrecaoAssinar }
];

var _formaPagamento = [
    { text: "Dinheiro", value: EnumFormaPagamentoNFCe.fpDinheiro },
    { text: "Cheque", value: EnumFormaPagamentoNFCe.fpCheque },
    { text: "Cartão de Crédito", value: EnumFormaPagamentoNFCe.fpCartaoCredito },
    { text: "Cartão de Débito", value: EnumFormaPagamentoNFCe.fpCartaoDebito },
    { text: "Credito de Loja", value: EnumFormaPagamentoNFCe.fpCreditoLoja },
    { text: "Vale Alimentação", value: EnumFormaPagamentoNFCe.fpValeAlimentacao },
    { text: "Vale Refeição", value: EnumFormaPagamentoNFCe.fpValeRefeicao },
    { text: "Vale Presente", value: EnumFormaPagamentoNFCe.fpValePresente },
    { text: "Vale Combustível", value: EnumFormaPagamentoNFCe.fpValeCombustivel },
    { text: "Duplicata Mercantil", value: EnumFormaPagamentoNFCe.fpDuplicataMercantil },
    { text: "Boleto Bancário", value: EnumFormaPagamentoNFCe.fpBoletoBancario },
    { text: "Depósito Bancário", value: EnumFormaPagamentoNFCe.fpDepositoBancario },
    { text: "PIX", value: EnumFormaPagamentoNFCe.fpPagamentoInstantaneoPIX },
    { text: "Transferência Bancária", value: EnumFormaPagamentoNFCe.fpTransferenciabancaria },
    { text: "Programa de Fidelidade", value: EnumFormaPagamentoNFCe.fpProgramadefidelidade },
    { text: "Sem Pagamento", value: EnumFormaPagamentoNFCe.fpSemPagamento },
    { text: "Outro", value: EnumFormaPagamentoNFCe.fpOutro }
];

var _parcelas = [
    { value: 1, text: "1X" },
    { value: 2, text: "2X" },
    { value: 3, text: "3X" },
    { value: 4, text: "4X" },
    { value: 5, text: "5X" },
    { value: 6, text: "6X" },
    { value: 7, text: "7X" },
    { value: 8, text: "8X" },
    { value: 9, text: "9X" },
    { value: 10, text: "10X" },
    { value: 11, text: "11X" },
    { value: 12, text: "12X" },
    { value: 13, text: "13X" },
    { value: 14, text: "14X" },
    { value: 15, text: "15X" },
    { value: 16, text: "16X" },
    { value: 17, text: "17X" },
    { value: 18, text: "18X" },
    { value: 19, text: "19X" },
    { value: 20, text: "20X" },
    { value: 21, text: "21X" },
    { value: 22, text: "22X" },
    { value: 23, text: "23X" },
    { value: 24, text: "24X" },
    { value: 25, text: "25X" },
    { value: 26, text: "26X" },
    { value: 27, text: "27X" },
    { value: 28, text: "28X" },
    { value: 29, text: "29X" },
    { value: 30, text: "30X" },
    { value: 31, text: "31X" },
    { value: 32, text: "32X" },
    { value: 33, text: "33X" },
    { value: 34, text: "34X" },
    { value: 35, text: "35X" },
    { value: 36, text: "36X" },
    { value: 37, text: "37X" },
    { value: 38, text: "38X" },
    { value: 39, text: "39X" },
    { value: 40, text: "40X" },
    { value: 41, text: "41X" },
    { value: 42, text: "42X" },
    { value: 43, text: "43X" },
    { value: 44, text: "44X" },
    { value: 45, text: "45X" },
    { value: 46, text: "46X" },
    { value: 47, text: "47X" },
    { value: 48, text: "48X" },
    { value: 49, text: "49X" },
    { value: 50, text: "50X" }
];

var PesquisaNotaFiscalConsumidor = function () {
    this.NumeroInicial = PropertyEntity({ text: "Número Inicial: ", getType: typesKnockout.int });
    this.NumeroFinal = PropertyEntity({ text: "Número Final: ", getType: typesKnockout.int });
    this.Serie = PropertyEntity({ text: "Série: ", getType: typesKnockout.int });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.Chave = PropertyEntity({ text: "Chave: ", maxlength: 44 });
    this.CnpjCpfPessoa = PropertyEntity({ text: "CPF / CNPJ: ", maxlength: 50, getType: typesKnockout.cpfCnpj });
    this.NomePessoa = PropertyEntity({ text: "Nome Pessoa: " });
    this.Status = PropertyEntity({ val: ko.observable(0), options: _statusNFe, def: 0, text: "Status: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridNotaFiscalConsumidor.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() === true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var NotaFiscalConsumidor = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.BuscarCliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente: ", idBtnSearch: guid(), visible: true });
    this.CPFConsumidor = PropertyEntity({ text: "CPF / CNPJ: ", required: false, maxlength: 50, getType: typesKnockout.cpfCnpj });
    this.NomeConsumidor = PropertyEntity({ text: "Nome: ", required: false, maxlength: 120 });

    this.ListaItens = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid(), list: new Array() });
    this.Itens = PropertyEntity({ type: types.map, required: false, text: "Itens NFC-e", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true) });
    this.TipoItem = PropertyEntity({ val: ko.observable(1), def: 1, options: _tipoItem, text: "Tipo Item:", required: false, enable: ko.observable(true), eventChange: TipoItemChange });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto:", idBtnSearch: guid(), required: false, enable: ko.observable(true), visible: ko.observable(true) });
    this.Servico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Serviço:", idBtnSearch: guid(), required: false, enable: ko.observable(true), visible: ko.observable(false) });
    this.QuantidadeItem = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Quantidade:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: _casasQuantidadeProduto, allowZero: true } });
    this.ValorUnitarioItem = PropertyEntity({ def: "0,0000", val: ko.observable("0,0000"), text: "Valor Unitário:", getType: typesKnockout.decimal, maxlength: 22, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: _casasValorProduto, allowZero: true } });
    this.ValorTotalItem = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor Total:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.AdicionarItem = PropertyEntity({ eventClick: AdicionarItemClick, type: types.event, text: "Adicionar Item", visible: ko.observable(true), enable: ko.observable(true) });

    this.ValorTotal = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor Total:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.ValorPago = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor Pago:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorTroco = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor Troco:", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });

    this.QuantidadeParcelas = PropertyEntity({ val: ko.observable(1), options: _parcelas, def: 1, text: "*Qtd. Parcelas: ", required: false });
    this.IntervaloDeDias = PropertyEntity({ text: "*Intervalo de Dias (Ex.: 20.30.40 para intervalos diferentes): ", required: false, maxlength: 100, getType: typesKnockout.string });
    this.DataPrimeiroVencimento = PropertyEntity({ getType: typesKnockout.date, text: "*Data Primeiro Vencimento:", required: false, enable: ko.observable(true) });
    this.TipoArredondamento = PropertyEntity({ val: ko.observable(EnumTipoArredondamento.PrimeiroItem), options: EnumTipoArredondamento.ObterOpcoes(), def: EnumTipoArredondamento.PrimeiroItem, text: "*Tipo Arredonda.: " });
    this.GerarParcelas = PropertyEntity({ eventClick: AdicionarParcelasClick, type: types.event, text: "Gerar Parcelas", visible: ko.observable(true), enable: ko.observable(true) });
    this.ParcelasNFCe = PropertyEntity({ type: types.local, id: guid() });
    this.ListaParcelas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid(), list: new Array() });
    this.CodigosPedidoVenda = PropertyEntity({ getType: typesKnockout.string, enable: ko.observable(true), val: ko.observable("") });

    this.FormaPagamento = PropertyEntity({ val: ko.observable(EnumFormaPagamentoNFCe.fpDinheiro), def: EnumFormaPagamentoNFCe.fpDinheiro, options: _formaPagamento, text: "Forma Pagamento:", required: false, enable: ko.observable(true) });
    this.ImportarPedido = PropertyEntity({ type: types.map, required: false, text: "Importar Pedido", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true) });

    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar Venda", visible: ko.observable(true), enable: ko.observable(true) });
    this.Finalizar = PropertyEntity({ eventClick: FinalizarClick, type: types.event, text: "Finalizar Venda", visible: ko.observable(true), enable: ko.observable(true) });
};

var JustificativaCancelamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataEmissao = PropertyEntity({ getType: typesKnockout.dateTime, text: "*Data de Emissão:", required: true, enable: ko.observable(true), val: ko.observable(Global.DataAtual()), def: ko.observable(Global.DataAtual()) });
    this.Justificativa = PropertyEntity({ getType: typesKnockout.string, required: true, maxlength: 300, text: "*Justificativa do Cancelamento:" });

    this.Cancelar = PropertyEntity({ type: types.event, eventClick: CancelarNotaFiscalClick, text: "Cancelar NF-e", visible: ko.observable(true), required: true });
};

var DetalheParcela = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Sequencia = PropertyEntity({ getType: typesKnockout.int, required: false, text: "Sequência:", maxlength: 10, enable: ko.observable(false) });
    this.Valor = PropertyEntity({ getType: typesKnockout.decimal, required: true, text: "*Valor:", maxlength: 10, enable: ko.observable(true) });
    this.DataEmissao = PropertyEntity({ getType: typesKnockout.date, required: false, text: "Data Emissão:", enable: ko.observable(false) });
    this.DataVencimento = PropertyEntity({ getType: typesKnockout.date, required: true, text: "*Data Vencimento:", enable: ko.observable(true) });
    this.FormaTitulo = PropertyEntity({ val: ko.observable(EnumFormaTitulo.Outros), options: EnumFormaTitulo.obterOpcoes(), text: "*Forma do Título: ", def: EnumFormaTitulo.Outros, required: false, enable: ko.observable(true) });

    this.SalvarParcela = PropertyEntity({ type: types.event, eventClick: SalvarParcelaClick, text: "Salvar Parcela", visible: ko.observable(true), enable: ko.observable(true) });
};

//*******EVENTOS*******

function loadNotaFiscalConsumidor() {
    HeaderAuditoria("NotaFiscal", {});
    BuscarDadosPadroesEmpresa();
}

function BuscarDadosPadroesEmpresa() {
    executarReST("NotaFiscalEletronica/BuscarDadosEmpresa", null, function (r) {
        if (r.Success) {
            _casasQuantidadeProduto = r.Data.CasasQuantidadeProdutoNFe;
            _casasValorProduto = r.Data.CasasValorProdutoNFe;
            _emitirVendaPrazoNFCe = r.Data.EmitirVendaPrazoNFCe;

            _pesquisaNotaFiscalConsumidor = new PesquisaNotaFiscalConsumidor();
            KoBindings(_pesquisaNotaFiscalConsumidor, "knockoutPesquisaNotaFiscalConsumidor", false, _pesquisaNotaFiscalConsumidor.Pesquisar.id);

            _notaFiscalConsumidor = new NotaFiscalConsumidor();
            KoBindings(_notaFiscalConsumidor, "knockoutCadastroNotaFiscalConsumidor");

            _justificativaCancelamento = new JustificativaCancelamento();
            KoBindings(_justificativaCancelamento, "knoutModalCancelarNFe");

            _detalheParcelaNotaFiscalConsumidor = new DetalheParcela();
            KoBindings(_detalheParcelaNotaFiscalConsumidor, "knoutDetalheParcela");

            _notaFiscalConsumidor.ListaItens.list = new Array();
            _notaFiscalConsumidor.ListaParcelas.list = new Array();

            new BuscarProdutoTMS(_notaFiscalConsumidor.Produto, function (data) {
                _notaFiscalConsumidor.Produto.codEntity(data.Codigo);
                _notaFiscalConsumidor.Produto.val(data.Descricao);
                if (data.ValorVenda != null && data.ValorVenda != "") {
                    _notaFiscalConsumidor.ValorUnitarioItem.val(data.ValorVenda);
                    if (r.Data.CasasQuantidadeProdutoNFe == 0)
                        _notaFiscalConsumidor.QuantidadeItem.val("1");
                    else if (r.Data.CasasQuantidadeProdutoNFe == 1)
                        _notaFiscalConsumidor.QuantidadeItem.val("1,0");
                    else if (r.Data.CasasQuantidadeProdutoNFe == 2)
                        _notaFiscalConsumidor.QuantidadeItem.val("1,00");
                    else if (r.Data.CasasQuantidadeProdutoNFe == 3)
                        _notaFiscalConsumidor.QuantidadeItem.val("1,000");
                    else
                        _notaFiscalConsumidor.QuantidadeItem.val("1,0000");
                    CalcularTotalItem();
                }
                $("#" + _notaFiscalConsumidor.QuantidadeItem.id).focus();
            });
            new BuscarServicoTMS(_notaFiscalConsumidor.Servico, function (data) {
                _notaFiscalConsumidor.Servico.codEntity(data.Codigo);
                _notaFiscalConsumidor.Servico.val(data.Descricao);
                if (data.ValorVenda != null && data.ValorVenda != "") {
                    _notaFiscalConsumidor.ValorUnitarioItem.val(data.ValorVenda);
                    if (r.Data.CasasQuantidadeProdutoNFe == 0)
                        _notaFiscalConsumidor.QuantidadeItem.val("1");
                    else if (r.Data.CasasQuantidadeProdutoNFe == 1)
                        _notaFiscalConsumidor.QuantidadeItem.val("1,0");
                    else if (r.Data.CasasQuantidadeProdutoNFe == 2)
                        _notaFiscalConsumidor.QuantidadeItem.val("1,00");
                    else if (r.Data.CasasQuantidadeProdutoNFe == 3)
                        _notaFiscalConsumidor.QuantidadeItem.val("1,000");
                    else
                        _notaFiscalConsumidor.QuantidadeItem.val("1,0000");
                    CalcularTotalItem();
                }
                $("#" + _notaFiscalConsumidor.QuantidadeItem.id).focus();
            });

            new BuscarClientes(_notaFiscalConsumidor.BuscarCliente, function (data) {
                _notaFiscalConsumidor.CPFConsumidor.val(data.CPF_CNPJ_SemFormato);
                _notaFiscalConsumidor.NomeConsumidor.val(data.Nome);
            });

            buscarNotaFiscalConsumidors();
            $("#" + _notaFiscalConsumidor.CPFConsumidor.id).focus();

            var editarItem = { descricao: "Editar", id: guid(), metodo: EditarItemClick, icone: "" };
            var excluirItem = { descricao: "Excluir", id: guid(), metodo: ExcluirItemClick, icone: "" };

            var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [editarItem, excluirItem] };

            var header = [
                { data: "Codigo", visible: false },
                { data: "CodigoProduto", visible: false },
                { data: "CodigoServico", visible: false },
                { data: "Descricao", title: "Descrição Item", width: "40%" },
                { data: "Quantidade", title: "Quantidade", width: "15%" },
                { data: "ValorUnitario", title: "Val. Unit.", width: "15%" },
                { data: "ValorTotal", title: "Val. Total", width: "15%" }];

            _gridItensNotaFiscalConsumidor = new BasicDataTable(_notaFiscalConsumidor.Itens.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });
            _notaFiscalConsumidor.Itens.basicTable = _gridItensNotaFiscalConsumidor;

            recarregarGridListaItens();

            var editarParcela = { descricao: "Editar", id: guid(), metodo: ObterDetalheClick, icone: "" };
            var excluirParcela = { descricao: "Excluir", id: guid(), metodo: ExcluirParcelaClick, icone: "" };

            var menuOpcoesParcelas = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [editarParcela, excluirParcela] };

            var header = [
                { data: "Codigo", visible: false },
                { data: "Sequencia", visible: false },
                { data: "Parcela", title: "Parcela", width: "15%" },
                { data: "Valor", title: "Valor", width: "15%" },
                { data: "DataEmissao", title: "Data Emissão", width: "20%" },
                { data: "DataVencimento", title: "Data Vencimento", width: "20%" },
                { data: "FormaTitulo", visible: false }
            ];

            _gridParcelasNotaFiscalConsumidor = new BasicDataTable(_notaFiscalConsumidor.ParcelasNFCe.id, header, menuOpcoesParcelas, { column: 1, dir: orderDir.asc }, null, 10);
            _notaFiscalConsumidor.ParcelasNFCe.basicTable = _gridParcelasNotaFiscalConsumidor;

            recarregarGridListaParcelas();

            CarregarGridPedidos();

            new BuscarPedidosVendas(_notaFiscalConsumidor.ImportarPedido, retornoPedidosVendas, null, _gridPedidos, EnumStatusPedidoVenda.Finalizada);

            if (_emitirVendaPrazoNFCe == false)
                $("#litabParcelamento").hide();

            if (r.Data.CasasQuantidadeProdutoNFe == 0) {
                _notaFiscalConsumidor.QuantidadeItem.def = "0";
                _notaFiscalConsumidor.QuantidadeItem.val("0");
                _notaFiscalConsumidor.QuantidadeItem.configDecimal = { precision: 0, allowZero: true };
            } else if (r.Data.CasasQuantidadeProdutoNFe == 1) {
                _notaFiscalConsumidor.QuantidadeItem.def = "0,0";
                _notaFiscalConsumidor.QuantidadeItem.val("0,0");
                _notaFiscalConsumidor.QuantidadeItem.configDecimal = { precision: 1, allowZero: true };
            } else if (r.Data.CasasQuantidadeProdutoNFe == 2) {
                _notaFiscalConsumidor.QuantidadeItem.def = "0,00";
                _notaFiscalConsumidor.QuantidadeItem.val("0,00");
                _notaFiscalConsumidor.QuantidadeItem.configDecimal = { precision: 2, allowZero: true };
            } else if (r.Data.CasasQuantidadeProdutoNFe == 3) {
                _notaFiscalConsumidor.QuantidadeItem.def = "0,000";
                _notaFiscalConsumidor.QuantidadeItem.val("0,000");
                _notaFiscalConsumidor.QuantidadeItem.configDecimal = { precision: 3, allowZero: true };
            } else {
                _notaFiscalConsumidor.QuantidadeItem.def = "0,0000";
                _notaFiscalConsumidor.QuantidadeItem.val("0,0000");
                _notaFiscalConsumidor.QuantidadeItem.configDecimal = { precision: 4, allowZero: true };
            }

            if (r.Data.CasasValorProdutoNFe == 0) {
                _notaFiscalConsumidor.ValorUnitarioItem.def = "0";
                _notaFiscalConsumidor.ValorUnitarioItem.val("0");
                _notaFiscalConsumidor.ValorUnitarioItem.configDecimal = { precision: 0, allowZero: true };
            } else if (r.Data.CasasValorProdutoNFe == 1) {
                _notaFiscalConsumidor.ValorUnitarioItem.def = "0,0";
                _notaFiscalConsumidor.ValorUnitarioItem.val("0,0");
                _notaFiscalConsumidor.ValorUnitarioItem.configDecimal = { precision: 1, allowZero: true };
            } else if (r.Data.CasasValorProdutoNFe == 2) {
                _notaFiscalConsumidor.ValorUnitarioItem.def = "0,00";
                _notaFiscalConsumidor.ValorUnitarioItem.val("0,00");
                _notaFiscalConsumidor.ValorUnitarioItem.configDecimal = { precision: 2, allowZero: true };
            } else if (r.Data.CasasValorProdutoNFe == 3) {
                _notaFiscalConsumidor.ValorUnitarioItem.def = "0,000";
                _notaFiscalConsumidor.ValorUnitarioItem.val("0,000");
                _notaFiscalConsumidor.ValorUnitarioItem.configDecimal = { precision: 3, allowZero: true };
            } else if (r.Data.CasasValorProdutoNFe == 4) {
                _notaFiscalConsumidor.ValorUnitarioItem.def = "0,0000";
                _notaFiscalConsumidor.ValorUnitarioItem.val("0,0000");
                _notaFiscalConsumidor.ValorUnitarioItem.configDecimal = { precision: 4, allowZero: true };
            } else if (r.Data.CasasValorProdutoNFe == 5) {
                _notaFiscalConsumidor.ValorUnitarioItem.def = "0,00000";
                _notaFiscalConsumidor.ValorUnitarioItem.val("0,00000");
                _notaFiscalConsumidor.ValorUnitarioItem.configDecimal = { precision: 5, allowZero: true };
            } else if (r.Data.CasasValorProdutoNFe == 6) {
                _notaFiscalConsumidor.ValorUnitarioItem.def = "0,000000";
                _notaFiscalConsumidor.ValorUnitarioItem.val("0,000000");
                _notaFiscalConsumidor.ValorUnitarioItem.configDecimal = { precision: 6, allowZero: true };
            } else if (r.Data.CasasValorProdutoNFe == 7) {
                _notaFiscalConsumidor.ValorUnitarioItem.def = "0,0000000";
                _notaFiscalConsumidor.ValorUnitarioItem.val("0,0000000");
                _notaFiscalConsumidor.ValorUnitarioItem.configDecimal = { precision: 7, allowZero: true };
            } else if (r.Data.CasasValorProdutoNFe == 8) {
                _notaFiscalConsumidor.ValorUnitarioItem.def = "0,00000000";
                _notaFiscalConsumidor.ValorUnitarioItem.val("0,00000000");
                _notaFiscalConsumidor.ValorUnitarioItem.configDecimal = { precision: 8, allowZero: true };
            } else if (r.Data.CasasValorProdutoNFe == 9) {
                _notaFiscalConsumidor.ValorUnitarioItem.def = "0,000000000";
                _notaFiscalConsumidor.ValorUnitarioItem.val("0,000000000");
                _notaFiscalConsumidor.ValorUnitarioItem.configDecimal = { precision: 9, allowZero: true };
            } else if (r.Data.CasasValorProdutoNFe == 10) {
                _notaFiscalConsumidor.ValorUnitarioItem.def = "0,0000000000";
                _notaFiscalConsumidor.ValorUnitarioItem.val("0,0000000000");
                _notaFiscalConsumidor.ValorUnitarioItem.configDecimal = { precision: 10, allowZero: true };
            } else {
                _notaFiscalConsumidor.ValorUnitarioItem.def = "0,00000";
                _notaFiscalConsumidor.ValorUnitarioItem.val("0,00000");
                _notaFiscalConsumidor.ValorUnitarioItem.configDecimal = { precision: 5, allowZero: true };
            }

        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

//PARCELA
function AdicionarParcelasClick(data) {
    _notaFiscalConsumidor.IntervaloDeDias.required = true;
    _notaFiscalConsumidor.DataPrimeiroVencimento.required = true;
    var valido = ValidarCamposObrigatorios(_notaFiscalConsumidor);
    if (valido)
        valido = parseFloat(_notaFiscalConsumidor.ValorTotal.val().toString().replace(".", "").replace(",", ".")) > 0;
    if (valido) {
        _notaFiscalConsumidor.ListaParcelas.list = new Array;
        var quantidadeParcelas = _notaFiscalConsumidor.QuantidadeParcelas.val();
        var valorTotal = parseFloat(_notaFiscalConsumidor.ValorTotal.val().toString().replace(".", "").replace(",", ".")).toFixed(2);
        valorTotal = parseFloat(valorTotal);
        var valorParcela = (valorTotal / quantidadeParcelas).toFixed(2);
        valorParcela = parseFloat(valorParcela);
        var valorDiferenca = valorTotal - (valorParcela * quantidadeParcelas).toFixed(2);
        valorDiferenca = parseFloat(valorDiferenca);
        var dataUltimaParcela = _notaFiscalConsumidor.DataPrimeiroVencimento.val();

        var x = _notaFiscalConsumidor.IntervaloDeDias.val();
        if (x.indexOf(".") >= 0) {
            var arrayDias = x.split(".");
            if (arrayDias.length != quantidadeParcelas) {
                exibirMensagem(tipoMensagem.atencao, "Intervalo de Dias", "As quantidades das parcelas não estão de acordo com o intervalo de dias informado!");
                return;
            }
            for (var i = 0; i < arrayDias.length; i++) {
                if (!parseInt(arrayDias[i]) > 0 || parseInt(arrayDias[i]) == NaN) {
                    exibirMensagem(tipoMensagem.atencao, "Intervalo de Dias", "O intervalo de dias está fora do formato desejado!");
                    return;
                }
            }
        } else {
            var arrayDias = new Array;
            arrayDias[0] = x;
            if (!parseInt(arrayDias[0]) > 0 || parseInt(arrayDias[0]) == NaN) {
                exibirMensagem(tipoMensagem.atencao, "Intervalo de Dias", "O intervalo de dias está fora do formato desejado!");
                return;
            }
        }

        var tipoArredondamento = _notaFiscalConsumidor.TipoArredondamento.val();

        for (var i = 0; i < quantidadeParcelas; i++) {

            var valor = 0;
            if (i == 0 && tipoArredondamento == EnumTipoArredondamento.PrimeiroItem)
                valor = (valorParcela + valorDiferenca).toFixed(2);
            else if ((i + 1) == quantidadeParcelas && tipoArredondamento == EnumTipoArredondamento.UltimoItem)
                valor = (valorParcela + valorDiferenca).toFixed(2);
            else
                valor = valorParcela.toFixed(2);

            if (i > 0) {
                dataUltimaParcela = dataUltimaParcela.substr(6, 4) + "/" + dataUltimaParcela.substr(3, 2) + "/" + dataUltimaParcela.substr(0, 2);
                var dataVencimento = new Date(dataUltimaParcela);
                if (arrayDias.length > 1)
                    dataVencimento.setDate(dataVencimento.getDate() + parseInt(arrayDias[i]));
                else
                    dataVencimento.setDate(dataVencimento.getDate() + parseInt(arrayDias[0]));

                var yyyy = dataVencimento.getFullYear().toString();
                var mm = (dataVencimento.getMonth() + 1).toString();
                var dd = dataVencimento.getDate().toString();

                dataUltimaParcela = (dd[1] ? dd : "0" + dd[0]) + "/" + (mm[1] ? mm : "0" + mm[0]) + "/" + yyyy;
            }

            _notaFiscalConsumidor.ListaParcelas.list.push({
                Codigo: guid(),
                Sequencia: i + 1,
                DataEmissao: moment().format("DD/MM/YYYY"),
                Parcela: i + 1,
                Valor: mvalor(valor.replace(".", ",")),
                DataVencimento: dataUltimaParcela,
                FormaTitulo: EnumFormaTitulo.Outros
            });
        }

        recarregarGridListaParcelas();
        LimparCamposParcelas();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios, e verifique o total da nota!");
    }
    _notaFiscalConsumidor.IntervaloDeDias.required = false;
    _notaFiscalConsumidor.DataPrimeiroVencimento.required = false;
}

function SalvarParcelaClick() {
    if (ValidarCamposObrigatorios(_detalheParcelaNotaFiscalConsumidor)) {

        for (var i = 0; i < _notaFiscalConsumidor.ListaParcelas.list.length; i++) {
            if (_detalheParcelaNotaFiscalConsumidor.Codigo.val() == _notaFiscalConsumidor.ListaParcelas.list[i].Codigo) {
                _notaFiscalConsumidor.ListaParcelas.list.splice(i, 1);
                break;
            }
        }

        _notaFiscalConsumidor.ListaParcelas.list.push({
            Codigo: _detalheParcelaNotaFiscalConsumidor.Codigo.val(),
            Sequencia: _detalheParcelaNotaFiscalConsumidor.Sequencia.val(),
            DataEmissao: _detalheParcelaNotaFiscalConsumidor.DataEmissao.val(),
            Parcela: _detalheParcelaNotaFiscalConsumidor.Sequencia.val(),
            Valor: _detalheParcelaNotaFiscalConsumidor.Valor.val(),
            DataVencimento: _detalheParcelaNotaFiscalConsumidor.DataVencimento.val(),
            FormaTitulo: _detalheParcelaNotaFiscalConsumidor.FormaTitulo.val()
        });

        recarregarGridListaParcelas();

        LimparCampos(_detalheParcelaNotaFiscalConsumidor);
        Global.fecharModal('divDetalheParcela');

    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios.");
    }
}

function recarregarGridListaParcelas() {
    var data = new Array();

    $.each(_notaFiscalConsumidor.ListaParcelas.list, function (i, listaParcela) {
        var listaParcelaGrid = new Object();

        listaParcelaGrid.Codigo = listaParcela.Codigo;
        listaParcelaGrid.Sequencia = listaParcela.Sequencia;
        listaParcelaGrid.Parcela = listaParcela.Parcela;
        listaParcelaGrid.Valor = listaParcela.Valor;
        listaParcelaGrid.DataEmissao = listaParcela.DataEmissao;
        listaParcelaGrid.DataVencimento = listaParcela.DataVencimento;
        listaParcelaGrid.FormaTitulo = listaParcela.FormaTitulo;

        data.push(listaParcelaGrid);
    });

    _gridParcelasNotaFiscalConsumidor.CarregarGrid(data);
}

function LimparCamposParcelas() {
    _notaFiscalConsumidor.QuantidadeParcelas.val(1);
    _notaFiscalConsumidor.QuantidadeParcelas.def = 1;
    _notaFiscalConsumidor.IntervaloDeDias.val("");
    _notaFiscalConsumidor.DataPrimeiroVencimento.val("");
    _notaFiscalConsumidor.TipoArredondamento.val(EnumTipoArredondamento.PrimeiroItem);
    _notaFiscalConsumidor.TipoArredondamento.def = EnumTipoArredondamento.PrimeiroItem;
}

function ObterDetalheClick(parcela) {
    LimparCampos(_detalheParcelaNotaFiscalConsumidor);

    if (parcela.Codigo != "") {
        var data =
        {
            Codigo: parcela.Codigo,
            Sequencia: parcela.Sequencia,
            Valor: parcela.Valor,
            DataEmissao: parcela.DataEmissao,
            DataVencimento: parcela.DataVencimento,
            FormaTitulo: parcela.FormaTitulo
        };
        var dataParcela = { Data: data };
        PreencherObjetoKnout(_detalheParcelaNotaFiscalConsumidor, dataParcela);
        Global.abrirModal('divDetalheParcela');
    }
}

function ExcluirParcelaClick(data) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a Parcela " + data.Parcela + "?", function () {
        $.each(_notaFiscalConsumidor.ListaParcelas.list, function (i, listaParcela) {
            if (data.Codigo == listaParcela.Codigo) {
                _notaFiscalConsumidor.ListaParcelas.list.splice(i, 1);
                return false;
            }
        });

        recarregarGridListaParcelas();
    });
}

function preencherListaParcelasSelecao() {
    _notaFiscalConsumidor.ListaParcelas.list = new Array();

    var parcelas = new Array();

    $.each(_notaFiscalConsumidor.ParcelasNFCe.basicTable.BuscarRegistros(), function (i, parcela) {
        parcelas.push({ Parcela: parcela });
    });

    _notaFiscalConsumidor.ListaParcelas.val(JSON.stringify(parcelas));
}

// ITEM
function recarregarGridListaItens() {
    var data = new Array();

    $.each(_notaFiscalConsumidor.ListaItens.list, function (i, listaItem) {
        var listaItemGrid = new Object();

        listaItemGrid.Codigo = listaItem.Codigo;
        listaItemGrid.CodigoProduto = listaItem.CodigoProduto;
        listaItemGrid.CodigoServico = listaItem.CodigoServico;
        listaItemGrid.Descricao = listaItem.Descricao;
        listaItemGrid.Quantidade = listaItem.Quantidade;
        listaItemGrid.ValorUnitario = listaItem.ValorUnitario;
        listaItemGrid.ValorTotal = listaItem.ValorTotal;

        data.push(listaItemGrid);
    });

    _gridItensNotaFiscalConsumidor.CarregarGrid(data);
}

function ExcluirItemClick(data) {
    if (_notaFiscalConsumidor.Codigo.val() > 0) {
        exibirMensagem("atencao", "Item em Edição", "Por favor, verifique os itens pois existe um em edição.");
        return
    }
    if (_notaFiscalConsumidor.TipoItem.val() == 1 && _notaFiscalConsumidor.Produto.val() != "") {
        exibirMensagem("atencao", "Item em Edição", "Por favor, verifique pois existe um produto sem salvar.");
        return
    }
    if (_notaFiscalConsumidor.TipoItem.val() == 2 && _notaFiscalConsumidor.Servico.val() != "") {
        exibirMensagem("atencao", "Item em Edição", "Por favor, verifique pois existe um serviço sem salvar.");
        return
    }

    $.each(_notaFiscalConsumidor.ListaItens.list, function (i, listaItem) {
        if (data.Codigo == listaItem.Codigo) {
            DiminuirValorTotal(parseFloat(listaItem.ValorTotal.toString().replace(".", "").replace(",", ".")));
            _notaFiscalConsumidor.ListaItens.list.splice(i, 1);
            return false;
        }
    });

    recarregarGridListaItens();
}

function EditarItemClick(data) {
    if (_notaFiscalConsumidor.Codigo.val() > 0) {
        exibirMensagem("atencao", "Item em Edição", "Por favor, verifique os itens pois existe um em edição.");
        return;
    }
    if (_notaFiscalConsumidor.TipoItem.val() == 1 && _notaFiscalConsumidor.Produto.val() != "") {
        exibirMensagem("atencao", "Item em Edição", "Por favor, verifique pois existe um produto sem salvar.");
        return;
    }
    if (_notaFiscalConsumidor.TipoItem.val() == 2 && _notaFiscalConsumidor.Servico.val() != "") {
        exibirMensagem("atencao", "Item em Edição", "Por favor, verifique pois existe um serviço sem salvar.");
        return;
    }

    DiminuirValorTotal(parseFloat(data.ValorTotal.toString().replace(".", "").replace(",", ".")));

    if (data.CodigoServico > 0)
        _notaFiscalConsumidor.TipoItem.val(2);
    else if (data.CodigoProduto > 0)
        _notaFiscalConsumidor.TipoItem.val(1);
    TipoItemChange();

    _notaFiscalConsumidor.Codigo.val(data.Codigo);
    _notaFiscalConsumidor.Produto.codEntity(data.CodigoProduto);
    _notaFiscalConsumidor.Produto.val(data.Descricao);
    _notaFiscalConsumidor.Servico.codEntity(data.CodigoServico);
    _notaFiscalConsumidor.Servico.val(data.Descricao);
    _notaFiscalConsumidor.QuantidadeItem.val(data.Quantidade);
    _notaFiscalConsumidor.ValorUnitarioItem.val(data.ValorUnitario);
    _notaFiscalConsumidor.ValorTotalItem.val(data.ValorTotal);
}

function AdicionarItemClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_notaFiscalConsumidor);
    _notaFiscalConsumidor.QuantidadeItem.requiredClass("form-control is-invalid");
    _notaFiscalConsumidor.ValorUnitarioItem.requiredClass("form-control is-invalid");
    _notaFiscalConsumidor.ValorTotalItem.requiredClass("form-control is-invalid");
    _notaFiscalConsumidor.Produto.requiredClass("form-control is-invalid");
    _notaFiscalConsumidor.Servico.requiredClass("form-control is-invalid");

    if (_notaFiscalConsumidor.ValorTotalItem.val() == "" || _notaFiscalConsumidor.ValorTotalItem.val() <= 0 || Globalize.parseFloat(_notaFiscalConsumidor.ValorTotalItem.val()) <= 0 || Globalize.parseFloat(_notaFiscalConsumidor.QuantidadeItem.val()) <= 0) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor informe a Quantidade/Valor Unitário/Valor Total do item!");
        _notaFiscalConsumidor.QuantidadeItem.requiredClass("form-control is-invalid");
        _notaFiscalConsumidor.ValorUnitarioItem.requiredClass("form-control is-invalid");
        _notaFiscalConsumidor.ValorTotalItem.requiredClass("form-control is-invalid");
        return;
    }
    if (_notaFiscalConsumidor.TipoItem.val() == 1 && _notaFiscalConsumidor.Produto.codEntity() == 0) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor informe o Produto!");
        _notaFiscalConsumidor.Produto.requiredClass("form-control is-invalid");
        return;
    } else if (_notaFiscalConsumidor.TipoItem.val() == 2 && _notaFiscalConsumidor.Servico.codEntity() == 0) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor informe o Serviço!");
        _notaFiscalConsumidor.Servico.requiredClass("form-control is-invalid");
        return;
    }

    if (valido) {
        if (_notaFiscalConsumidor.Codigo.val() > "0" && _notaFiscalConsumidor.Codigo.val() != undefined) {
            for (var i = 0; i < _notaFiscalConsumidor.ListaItens.list.length; i++) {
                if (_notaFiscalConsumidor.Codigo.val() == _notaFiscalConsumidor.ListaItens.list[i].Codigo) {
                    _notaFiscalConsumidor.ListaItens.list.splice(i, 1);
                    break;
                }
            }
        }
        _notaFiscalConsumidor.ListaItens.list.push({
            Codigo: guid(),
            CodigoProduto: _notaFiscalConsumidor.Produto.codEntity(),
            CodigoServico: _notaFiscalConsumidor.Servico.codEntity(),
            Descricao: _notaFiscalConsumidor.Produto.codEntity() > 0 ? _notaFiscalConsumidor.Produto.val() : _notaFiscalConsumidor.Servico.val(),
            Quantidade: _notaFiscalConsumidor.QuantidadeItem.val(),
            ValorUnitario: _notaFiscalConsumidor.ValorUnitarioItem.val(),
            ValorTotal: _notaFiscalConsumidor.ValorTotalItem.val()
        });

        AcrescentarValorTotal(parseFloat(_notaFiscalConsumidor.ValorTotalItem.val().toString().replace(".", "").replace(",", ".")));

        recarregarGridListaItens();

        if (_casasQuantidadeProduto == 0) {
            _notaFiscalConsumidor.QuantidadeItem.val("0");
        } else if (_casasQuantidadeProduto == 1) {
            _notaFiscalConsumidor.QuantidadeItem.val("0,0");
        } else if (_casasQuantidadeProduto == 2) {
            _notaFiscalConsumidor.QuantidadeItem.val("0,00");
        } else if (_casasQuantidadeProduto == 3) {
            _notaFiscalConsumidor.QuantidadeItem.val("0,000");
        } else {
            _notaFiscalConsumidor.QuantidadeItem.val("0,0000");
        }

        if (_casasValorProduto == 0) {
            _notaFiscalConsumidor.ValorUnitarioItem.val("0");
        } else if (_casasValorProduto == 1) {
            _notaFiscalConsumidor.ValorUnitarioItem.val("0,0");
        } else if (_casasValorProduto == 2) {
            _notaFiscalConsumidor.ValorUnitarioItem.val("0,00");
        } else if (_casasValorProduto == 3) {
            _notaFiscalConsumidor.ValorUnitarioItem.val("0,000");
        } else if (_casasValorProduto == 4) {
            _notaFiscalConsumidor.ValorUnitarioItem.val("0,0000");
        } else if (_casasValorProduto == 5) {
            _notaFiscalConsumidor.ValorUnitarioItem.val("0,00000");
        } else if (_casasValorProduto == 6) {
            _notaFiscalConsumidor.ValorUnitarioItem.val("0,000000");
        } else if (_casasValorProduto == 7) {
            _notaFiscalConsumidor.ValorUnitarioItem.val("0,0000000");
        } else if (_casasValorProduto == 8) {
            _notaFiscalConsumidor.ValorUnitarioItem.val("0,00000000");
        } else if (_casasValorProduto == 9) {
            _notaFiscalConsumidor.ValorUnitarioItem.val("0,000000000");
        } else if (_casasValorProduto == 10) {
            _notaFiscalConsumidor.ValorUnitarioItem.val("0,0000000000");
        } else {
            _notaFiscalConsumidor.ValorUnitarioItem.val("0,00000");
        }

        _notaFiscalConsumidor.ValorTotalItem.val("0,00");

        LimparCampoEntity(_notaFiscalConsumidor.Produto);
        LimparCampoEntity(_notaFiscalConsumidor.Servico);
        _notaFiscalConsumidor.TipoItem.val(1);
        TipoItemChange(e, sender);

        $("#" + _notaFiscalConsumidor.TipoItem.id).focus();

    } else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
}

function TipoItemChange(e, sender) {
    if (_notaFiscalConsumidor.TipoItem.val() == 1) {
        _notaFiscalConsumidor.Servico.required = false;
        _notaFiscalConsumidor.Servico.visible(false);
        LimparCampoEntity(_notaFiscalConsumidor.Servico);
        _notaFiscalConsumidor.Produto.required = false;
        _notaFiscalConsumidor.Produto.visible(true);
    }
    else {
        _notaFiscalConsumidor.Servico.required = false;
        _notaFiscalConsumidor.Servico.visible(true);

        _notaFiscalConsumidor.Produto.required = false;
        _notaFiscalConsumidor.Produto.visible(false);
        LimparCampoEntity(_notaFiscalConsumidor.Produto);
    }
}

function CalcularTotalItem() {
    var quantidade = parseFloat(_notaFiscalConsumidor.QuantidadeItem.val().toString().replace(".", "").replace(",", ".")).toFixed(_casasQuantidadeProduto);
    quantidade = parseFloat(quantidade);

    var valorUnitario = parseFloat(_notaFiscalConsumidor.ValorUnitarioItem.val().toString().replace(".", "").replace(",", ".")).toFixed(_casasValorProduto);
    valorUnitario = parseFloat(valorUnitario);

    if (quantidade > 0 && valorUnitario > 0) {
        var valorTotal = quantidade * valorUnitario;
        _notaFiscalConsumidor.ValorTotalItem.val(Globalize.format(valorTotal, "n2"));
    }
    else
        _notaFiscalConsumidor.ValorTotalItem.val("0,00");
}

function CalcularTroco() {
    var total = parseFloat(_notaFiscalConsumidor.ValorTotal.val().toString().replace(".", "").replace(",", ".")).toFixed(2);
    total = parseFloat(total);

    var valorPago = parseFloat(_notaFiscalConsumidor.ValorPago.val().toString().replace(".", "").replace(",", ".")).toFixed(4);
    valorPago = parseFloat(valorPago);

    _notaFiscalConsumidor.ValorTroco.val("0,00");

    if (total > 0 && valorPago > 0 && valorPago > total) {
        var valorTroco = valorPago - total;
        _notaFiscalConsumidor.ValorTroco.val(Globalize.format(valorTroco, "n2"));
    }
}

function CancelarClick(e, sender) {
    limparCamposNotaFiscalConsumidor();
}

function FinalizarClick(e, sender) {
    if (_notaFiscalConsumidor.Codigo.val() > 0) {
        exibirMensagem("atencao", "Item em Edição", "Por favor, verifique os itens pois existe um em edição.");
        return
    }
    if (_notaFiscalConsumidor.TipoItem.val() == 1 && _notaFiscalConsumidor.Produto.val() != "") {
        exibirMensagem("atencao", "Item em Edição", "Por favor, verifique pois existe um produto sem salvar.");
        return
    }
    if (_notaFiscalConsumidor.TipoItem.val() == 2 && _notaFiscalConsumidor.Servico.val() != "") {
        exibirMensagem("atencao", "Item em Edição", "Por favor, verifique pois existe um serviço sem salvar.");
        return
    }

    _notaFiscalConsumidor.ValorTotal.requiredClass("form-control is-invalid");
    _notaFiscalConsumidor.ValorPago.requiredClass("form-control is-invalid");

    if (_notaFiscalConsumidor.ValorTotal.val() == "" || _notaFiscalConsumidor.ValorTotal.val() <= 0 || Globalize.parseFloat(_notaFiscalConsumidor.ValorTotal.val()) <= 0) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor lance os itens para a nota fiscal!");
        _notaFiscalConsumidor.ValorTotal.requiredClass("form-control is-invalid");
        return;
    }

    if (_emitirVendaPrazoNFCe == false && (_notaFiscalConsumidor.ValorPago.val() == "" || _notaFiscalConsumidor.ValorPago.val() <= 0 || Globalize.parseFloat(_notaFiscalConsumidor.ValorPago.val()) <= 0)) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor informe o valor pago!");
        _notaFiscalConsumidor.ValorPago.requiredClass("form-control is-invalid");
        $("#" + _notaFiscalConsumidor.ValorPago.id).focus();
        return;
    }

    var total = parseFloat(_notaFiscalConsumidor.ValorTotal.val().toString().replace(".", "").replace(",", ".")).toFixed(2);
    total = parseFloat(total);

    var valorPago = parseFloat(_notaFiscalConsumidor.ValorPago.val().toString().replace(".", "").replace(",", ".")).toFixed(4);
    valorPago = parseFloat(valorPago);

    if (_emitirVendaPrazoNFCe == true && valorPago < total) {
        if (_notaFiscalConsumidor.ListaParcelas.list.length == 0) {
            valido = false;
            exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor informar pelo menos uma Parcela ou informar o valor pago igual ou maior que o valor total!");
            return;
        }
    }
    else if (valorPago < total) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor informe o valor pago igual ou maior que o valor total!");
        _notaFiscalConsumidor.ValorPago.requiredClass("form-control is-invalid");
        $("#" + _notaFiscalConsumidor.ValorPago.id).focus();
        return;
    }

    if (_emitirVendaPrazoNFCe == true && _notaFiscalConsumidor.ListaParcelas.list.length > 0) {
        var valorParcelas = 0;
        for (var i = 0; i < _notaFiscalConsumidor.ListaParcelas.list.length; i++) {
            var vlrParcela = parseFloat(_notaFiscalConsumidor.ListaParcelas.list[i].Valor.replace(".", "").replace(",", "."));
            valorParcelas = valorParcelas + vlrParcela;
        }

        if (valorParcelas > 0 && valorParcelas != total) {
            valido = false;
            exibirMensagem(tipoMensagem.atencao, "Parcelas", "Soma do valor das parcelas diferem do total da nota. Verifique!");
            return;
        }
    }

    var data = {
        NFCe: true,
        DataEmissao: Global.DataAtual()
    };

    executarReST("PlanoOrcamentario/ValidaPlanoOrcamentarioEmpresa", data, function (arg) {
        if (arg.Success) {
            if (arg.Data != true && arg.Data.Mensagem != "") {
                if (arg.Data.TipoLancamentoFinanceiroSemOrcamento == EnumTipoLancamentoFinanceiroSemOrcamento.Avisar) {
                    exibirConfirmacao("Confirmação", arg.Data.Mensagem, function () {
                        FinalizarEnviarClick(e, sender);
                    });
                } else
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            } else {
                FinalizarEnviarClick(e, sender);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function FinalizarEnviarClick(e, sender) {
    preencherListasSelecao();
    preencherListaParcelasSelecao();
    Salvar(e, "NotaFiscalConsumidor/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                var data = {
                    Codigo: arg.Data.Codigo
                };
                executarReST("NotaFiscalConsumidor/EnviarNFCe", data, function (arg) {
                    if (arg.Success) {
                        if (arg.Data) {
                            var data = {
                                Codigo: arg.Data.Codigo,
                                Chave: arg.Data.Chave
                            };
                            if (arg.Data.Status == EnumStatusNFe.Autorizado)
                                executarDownload("NotaFiscalConsumidor/DownloadDANFENFCe", data);
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "NFC-e emitida com sucesso");
                            limparCamposNotaFiscalConsumidor();
                            buscarNotaFiscalConsumidors();
                        } else {
                            exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 60000);
                            limparCamposNotaFiscalConsumidor();
                            buscarNotaFiscalConsumidors();
                        }
                    } else {
                        exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
                    }
                });
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function CancelarNotaFiscalClick(e) {
    if (ValidarCamposObrigatorios(e)) {
        if (e.Justificativa.val().length >= 15) {
            var dados = { Codigo: e.Codigo.val(), Justificativa: e.Justificativa.val(), DataEmissao: e.DataEmissao.val() };
            executarReST("NotaFiscalEletronica/CancelarNFe", dados, function (arg) {
                if (arg.Success) {
                    if (arg.Data != false) {
                        LimparCampos(_justificativaCancelamento);
                        Global.fecharModal('ModalCancelarNFe');
                        buscarNotaFiscalConsumidors();
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "NFC-e cancelado com sucesso");
                    } else {
                        exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 60000);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }
            });
        } else {
            exibirMensagem("atencao", "Campos Obrigatórios", "Por favor, informe mais que 15 caracteres.");
        }
    } else
        exibirMensagem("atencao", "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
}

//*******MÉTODOS*******

function AcrescentarValorTotal(valor) {
    var valorTotalNFeTotal = parseFloat(_notaFiscalConsumidor.ValorTotal.val().toString().replace(".", "").replace(",", "."));
    valorTotalNFeTotal = valorTotalNFeTotal + valor;
    _notaFiscalConsumidor.ValorTotal.val(Globalize.format(valorTotalNFeTotal, "n2"));
}

function DiminuirValorTotal(valor) {
    var valorTotalNFeTotal = parseFloat(_notaFiscalConsumidor.ValorTotal.val().toString().replace(".", "").replace(",", "."));
    valorTotalNFeTotal = valorTotalNFeTotal - valor;
    _notaFiscalConsumidor.ValorTotal.val(Globalize.format(valorTotalNFeTotal, "n2"));
}

function VisibilidadeAutorizadoCancelado(notaFiscalGrid) {
    return notaFiscalGrid.Status === EnumStatusNFe.Autorizado || notaFiscalGrid.Status === EnumStatusNFe.Cancelado;
}

function VisibilidadeCancelado(notaFiscalGrid) {
    return notaFiscalGrid.Status === EnumStatusNFe.Cancelado;
}

function buscarNotaFiscalConsumidors() {
    var enviarNFe = { descricao: "Autorizar NFC-e", id: guid(), metodo: enviarNotaFiscalConsumidor, icone: "" };
    var cancelarNFe = { descricao: "Cancelar NFC-e", id: guid(), metodo: cancelarNotaFiscalConsumidor, icone: "" };
    var baixarDANFE = { descricao: "Baixar DANFCE", id: guid(), metodo: baixarDANFENotaFiscalConsumidor, icone: "" };
    var baixarXML = { descricao: "Baixar XML NFC-e", id: guid(), metodo: baixarXMLNotaFiscalConsumidor, icone: "" };
    var baixarXMLCancelamento = { descricao: "Baixar XML Cancel.", id: guid(), metodo: baixarXMLCancelamentoNotaFiscalConsumidor, icone: "", visibilidade: VisibilidadeCancelado };
    var inutilizarNFe = { descricao: "Inutilizar NFC-e", id: guid(), metodo: inutilizarNotaFiscalConsumidor, icone: "" };
    var reenviarEmail = { descricao: "Reenviar e-mails", id: guid(), metodo: reenviarEmailNotaFiscalConsumidor, icone: "", visibilidade: VisibilidadeAutorizadoCancelado };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 10, opcoes: [enviarNFe, cancelarNFe, baixarDANFE, baixarXML, baixarXMLCancelamento, inutilizarNFe, reenviarEmail] };

    _gridNotaFiscalConsumidor = new GridView(_pesquisaNotaFiscalConsumidor.Pesquisar.idGrid, "NotaFiscalConsumidor/Pesquisa", _pesquisaNotaFiscalConsumidor, menuOpcoes, null);
    _gridNotaFiscalConsumidor.CarregarGrid();
}

function inutilizarNotaFiscalConsumidor(data) {
    if (data.Status == EnumStatusNFe.Rejeitado || data.Status == EnumStatusNFe.Emitido || data.Status == EnumStatusNFe.EmProcessamento || data.Status == EnumStatusNFe.AguardandoAssinar) {
        var dataEnvio = {
            Codigo: data.Codigo
        };
        executarReST("NotaFiscalConsumidor/InutilizarNFCe", dataEnvio, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "NFC-e inutilizada com sucesso");
                    buscarNotaFiscalConsumidors();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 60000);
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
            }
        });
    } else
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Esta nota fiscal se encontra com o status " + _statusNFe[data.Status].text, 60000);
}

function enviarNotaFiscalConsumidor(data) {
    if (data.Status == EnumStatusNFe.Rejeitado || data.Status == EnumStatusNFe.Emitido || data.Status == EnumStatusNFe.EmProcessamento || data.Status == EnumStatusNFe.AguardandoAssinar) {
        var dataEnvio = {
            NFCe: true,
            DataEmissao: data.DataEmissao,
            Codigo: data.Codigo
        };

        executarReST("PlanoOrcamentario/ValidaPlanoOrcamentarioEmpresa", dataEnvio, function (arg) {
            if (arg.Success) {
                if (arg.Data != true && arg.Data.Mensagem != "") {
                    if (arg.Data.TipoLancamentoFinanceiroSemOrcamento == EnumTipoLancamentoFinanceiroSemOrcamento.Avisar) {
                        exibirConfirmacao("Confirmação", arg.Data.Mensagem, function () {
                            enviarNotaFiscalConsumidorAprovada(dataEnvio);
                        });
                    } else
                        exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                } else {
                    enviarNotaFiscalConsumidorAprovada(dataEnvio);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    } else
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Esta nota fiscal se encontra com o status " + _statusNFe[data.Status].text, 60000);
}

function enviarNotaFiscalConsumidorAprovada(data) {
    var dataEnvio = {
        Codigo: data.Codigo
    };
    executarReST("NotaFiscalConsumidor/EnviarNFCe", dataEnvio, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                var data = {
                    Codigo: arg.Data.Codigo,
                    Chave: arg.Data.Chave
                };
                if (arg.Data.Status == EnumStatusNFe.Autorizado)
                    executarDownload("NotaFiscalConsumidor/DownloadDANFENFCe", data);
                exibirMensagem(tipoMensagem.ok, "Sucesso", "NFC-e emitida com sucesso");
                limparCamposNotaFiscalConsumidor();
                buscarNotaFiscalConsumidors();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 60000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function cancelarNotaFiscalConsumidor(data) {
    if (data.Status == EnumStatusNFe.Autorizado) {
        LimparCampos(_justificativaCancelamento);
        _justificativaCancelamento.DataEmissao.val(Global.DataHoraAtual());
        _justificativaCancelamento.Codigo.val(data.Codigo);
        Global.abrirModal('ModalCancelarNFe');
    } else
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Esta nota fiscal se encontra com o status " + _statusNFe[data.Status].text, 60000);
}

function baixarDANFENotaFiscalConsumidor(data) {
    if (data.Status == EnumStatusNFe.Autorizado || data.Status == EnumStatusNFe.Cancelado || data.Status == EnumStatusNFe.AguardandoCancelarAssinar || data.Status == EnumStatusNFe.AguardandoCartaCorrecaoAssinar) {
        var dataConsulta = {
            Codigo: data.Codigo,
            Chave: data.Chave
        };
        executarDownload("NotaFiscalConsumidor/DownloadDANFENFCe", dataConsulta);
    } else
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Esta nota fiscal se encontra com o status " + _statusNFe[data.Status].text, 60000);
}

function baixarXMLNotaFiscalConsumidor(data) {
    if (data.Status == EnumStatusNFe.Autorizado || data.Status == EnumStatusNFe.Cancelado || data.Status == EnumStatusNFe.AguardandoCancelarAssinar || data.Status == EnumStatusNFe.AguardandoCartaCorrecaoAssinar) {
        var dados = { Codigo: data.Codigo };
        executarDownload("NotaFiscalEletronica/DownloadXML", dados);
    } else
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Esta nota fiscal se encontra com o status " + _statusNFe[data.Status].text, 60000);
}

function baixarXMLCancelamentoNotaFiscalConsumidor(data) {
    if (data.Status === EnumStatusNFe.Cancelado) {
        var dados = { Codigo: data.Codigo };
        executarDownload("NotaFiscalEletronica/DownloadXMLCancelamento", dados);
    } else
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Esta nota fiscal se encontra com o status " + _statusNFePesquisa[data.Status].text, 60000);
}

function reenviarEmailNotaFiscalConsumidor(data) {
    if (data.Status === EnumStatusNFe.Autorizado || data.Status === EnumStatusNFe.Cancelado || data.Status === EnumStatusNFe.AguardandoCancelarAssinar || data.Status === EnumStatusNFe.AguardandoCartaCorrecaoAssinar) {
        var dados = { Codigo: data.Codigo };
        executarReST("NotaFiscalConsumidor/EnviarEmailNFCe", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Processo de envio de e-mail ativado.");
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 60000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    } else
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Esta nota fiscal se encontra com o status " + _statusNFePesquisa[data.Status].text, 60000);
}

function preencherListasSelecao() {
    _notaFiscalConsumidor.ListaItens.list = new Array();

    var itens = new Array();

    $.each(_notaFiscalConsumidor.Itens.basicTable.BuscarRegistros(), function (i, item) {
        itens.push({ Item: item });
    });

    _notaFiscalConsumidor.ListaItens.val(JSON.stringify(itens));
}

function limparCamposNotaFiscalConsumidor() {
    _notaFiscalConsumidor.ListaItens.list = new Array();
    recarregarGridListaItens();

    _notaFiscalConsumidor.ListaParcelas.list = new Array();
    recarregarGridListaParcelas();

    LimparCampos(_notaFiscalConsumidor);

    ResetarTabs();
    $("#" + _notaFiscalConsumidor.CPFConsumidor.id).focus();
}

function ResetarTabs() {
    $(".nav-tabs").each(function () {
        $(this).find("a:first").tab("show");
    });
}

function mvalor(v) {
    v = v.replace(/\D/g, "");
    v = v.replace(/(\d)(\d{8})$/, "$1.$2");
    v = v.replace(/(\d)(\d{5})$/, "$1.$2");

    v = v.replace(/(\d)(\d{2})$/, "$1,$2");
    return v;
}

function CarregarGridPedidos() {
    var pedidos = new Array();

    var header = [{ data: "Codigo", visible: false },
    { data: "Numero", visible: false },
    { data: "DataEmissao", visible: false },
    { data: "Cliente", visible: false },
    { data: "DescricaoTipo", visible: false },
    { data: "DescricaoStatus", visible: false },
    { data: "ValorTotal", visible: false }
    ];
    _gridPedidos = new BasicDataTable(_notaFiscalConsumidor.ImportarPedido.idGrid, header);
    _gridPedidos.CarregarGrid(pedidos);
}

function retornoPedidosVendas(pedidos) {
    limparCamposNotaFiscalConsumidor();
    var codigos = new Array();
    $.each(pedidos, function (i, pedido) {
        codigos.push({ Codigo: pedido.Codigo });
    });

    CarregarDadosPedido(codigos);
}

function CarregarDadosPedido(codigosPedidoVenda) {
    executarReST("NotaFiscalConsumidor/CarregarPedidoVendaPorCodigo", { Codigos: JSON.stringify(codigosPedidoVenda) }, function (r) {
        if (r.Success) {
            if (r.Data) {
                var data = r.Data;
                _notaFiscalConsumidor.CodigosPedidoVenda.val(JSON.stringify(codigosPedidoVenda));
                _notaFiscalConsumidor.CPFConsumidor.val(data.NFCe.Pessoa.CNPJCPFCliente);
                _notaFiscalConsumidor.NomeConsumidor.val(data.NFCe.Pessoa.NomeCliente);
                _notaFiscalConsumidor.ValorTotal.val(data.Totalizador.ValorTotal);

                if (data.ProdutosServicos != null) {
                    for (var i = 0; i < data.ProdutosServicos.length; i++) {
                        var item = data.ProdutosServicos[i];
                        _notaFiscalConsumidor.ListaItens.list.push({
                            Codigo: guid(),
                            CodigoProduto: item.CodigoProduto,
                            CodigoServico: item.CodigoServico,
                            Descricao: item.Descricao,
                            Quantidade: item.Qtd,
                            ValorUnitario: item.ValorUnitario,
                            ValorTotal: item.ValorTotal
                        });
                    }
                    recarregarGridListaItens();
                }
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso!", r.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
};
