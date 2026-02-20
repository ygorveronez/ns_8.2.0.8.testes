/*VeiculoMonitoramento.js*/
/// <reference path="../../../js/Global/Buscas.js" />
/// <reference path="../../../js/Global/Buscas.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/CRUD.js" />

//var _gridAtendimentoPedido;
var _pesquisaAtendimentoPedido;

/*
 * Declaração das Classes
 */
var PesquisaAtendimentoPedido = function () {

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            if (ValidarCamposObrigatorios(_pesquisaAtendimentoPedido)) {
                _pesquisaAtendimentoPedido.ExibirFiltros.visibleFade(false);
                pesquisar(1, false);
            }
            else
                exibirMensagemCamposObrigatorio();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    var dataDiaAnterior = moment().add(-1, 'days').format("DD/MM/YYYY");
    var dataDiaSeguinte = moment().add(1, 'days').format("DD/MM/YYYY");

    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date, val: ko.observable(dataDiaAnterior), def: dataDiaAnterior });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date, val: ko.observable(dataDiaSeguinte), def: dataDiaSeguinte });
    this.NumeroPedido = PropertyEntity({ text: ko.observable("Pedido:"), val: ko.observable(""), maxlength: 20 });
    this.PedidosComChat = PropertyEntity({ text: "Apenas pedidos com chat", val: ko.observable(true), visible: true, def: true });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid(), visible: ko.observable(true), cssClass: ko.observable("") });
    this.NotaFiscal = PropertyEntity({ val: ko.observable(""), def: "", text: "Número da NF:", configInt: { precision: 0, allowZero: false }, getType: typesKnockout.int, visible: ko.observable(true), cssClass: ko.observable("") });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}

var AtendimentoClientePedido = function () {
    this.Pedidos = PropertyEntity({ type: types.local, val: ko.observableArray([]) });
}

/*
 * Declaração das Funções de Inicialização
 */
function loadAtendimentoPedido() {
    _pesquisaAtendimentoPedido = new PesquisaAtendimentoPedido();
    KoBindings(_pesquisaAtendimentoPedido, "knockoutPesquisaAtendimentoPedido", false, _pesquisaAtendimentoPedido.Pesquisar.id);

    new BuscarClientes(_pesquisaAtendimentoPedido.Remetente);

    registrarComponente();
    loadGridAtendimentoPedidos();
    loadChatAtendimentoCliente();
    LoadConexaoSignalRMensagens();
}


function loadGridAtendimentoPedidos() {
    _atendimentoClientePedido = new AtendimentoClientePedido();
    KoBindings(_atendimentoClientePedido, "knoutAtendimentoPedido");

    pesquisar(1, false);
}

function pesquisar(page, paginou) {
    var itensPorPagina = 10;

    var data = RetornarObjetoPesquisa(_pesquisaAtendimentoPedido);

    data.inicio = itensPorPagina * (page - 1);
    data.limite = itensPorPagina;

    executarReST("AtendimentoPedidoCliente/PesquisaPedidosChat", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _atendimentoClientePedido.Pedidos.val(arg.Data.Pedidos);
                configurarPaginacao(page, paginou, arg, itensPorPagina);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
    });
}

function registrarComponente() {
    if (ko.components.isRegistered('pedido-card-container'))
        return;

    ko.components.register('pedido-card-container', {
        template: {
            element: "pedido-card-container"
        }
    });
}


function configurarPaginacao(page, paginou, retorno, itensPorPagina) {
    var clicouNoPaginar = false;

    if (!paginou) {
        if (retorno.QuantidadeRegistros > 0) {
            $("#divPaginationAtendimentoPedido").html('<ul style="float:right" id="paginacaoAtendimentoPedido" class="pagination"></ul>');
            var paginas = Math.ceil((retorno.QuantidadeRegistros / itensPorPagina));
            $('#paginacaoAtendimentoPedido').twbsPagination({
                first: 'Primeiro',
                prev: 'Anterior',
                next: 'Próximo',
                last: 'Último',
                totalPages: paginas,
                visiblePages: 5,
                onPageClick: function (event, page) {
                    if (clicouNoPaginar)
                        pesquisar(page, true);
                    clicouNoPaginar = true;
                }
            });
        }
        else
            $("#divPaginationAtendimentoPedido").html('<span>Nenhum Registro Encontrado</span>');
    }
}

/*
 * Declaração das Funções
 */
function exibirMensagemCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, informe os campos obrigatórios");
}


function finalizarAtendimento(pedido) {
    //limparModalChat();
    exibirConfirmacao("Finalizar Atendimento", "Você tem certeza que deseja Finalizar o atendimento?", function () {
        executarReST("AtendimentoPedidoCliente/Finalizar", { Pedido: pedido.Codigo }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                    pesquisar(1, false);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

