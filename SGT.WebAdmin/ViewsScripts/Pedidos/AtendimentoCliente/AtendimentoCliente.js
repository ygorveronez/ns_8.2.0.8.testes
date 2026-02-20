/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

var _atendimentoClientePesquisa,
    _atendimentoClientePedido;

var _cardativo;
var _dataChat;

var AtendimentoClientePesquisa = function () {

    this.CNPJDestinatario = PropertyEntity({ val: ko.observable(""), maxlength: 20 });
    this.NotaFiscal = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.int, def: "" });
    this.NumeroPedido = PropertyEntity({ val: ko.observable(""), maxlength: 20 });
    this.DataInicial = PropertyEntity({ getType: typesKnockout.date, val: ko.observable() });
    this.DataFinal = PropertyEntity({ getType: typesKnockout.date, val: ko.observable() });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;
}

var AtendimentoClientePedido = function () {
    this.Pedidos = PropertyEntity({ type: types.local, val: ko.observableArray([]) });
    this.classPedidos = PropertyEntity({ cssClass: ko.observable("col col-xl-12") });
}

function IniciarNovoChat() {
    _atendimentoClientePedido.classPedidos.cssClass("col col-xl-6");
    $("#knoutChatAtendimentoPedido").show();

    Global.fecharModal('divSolicitarNovoAtendimentoChat');
}

function closeSolicitarNovoAtendimento() {
    Global.fecharModal('divSolicitarNovoAtendimentoChat');
    limparModalChat();
}

function loadAtendimentoCliente() {
    _atendimentoClientePesquisa = new AtendimentoClientePesquisa();
    KoBindings(_atendimentoClientePesquisa, "knoutPesquisaAtendimentoCliente");

    _atendimentoClientePedido = new AtendimentoClientePedido();
    KoBindings(_atendimentoClientePedido, "knoutAtendimentoClientePedido");

    loadChatAtendimentoCliente();
    LoadConexaoSignalRMensagens();

    setarEventos();
    adicionarObservadores();
    registrarComponente();

    if (_CONFIGURACAO_TMS.DiasAnterioresPesquisaCarga > 0) 
        _atendimentoClientePesquisa.DataInicial.val(Global.Data(EnumTipoOperacaoDate.Subtract, _CONFIGURACAO_TMS.DiasAnterioresPesquisaCarga, EnumTipoOperacaoObjetoDate.Days));

    pesquisar(1, false);
}

function adicionarObservadores() {
    _atendimentoClientePesquisa.DataInicial.val.subscribe(onDataChange);
    _atendimentoClientePesquisa.DataFinal.val.subscribe(onDataChange);
}

function onDataChange(val) {
    if ("__/__/____" == val) return;
    pesquisar();
}

function setarEventos() {
    for (prop in _atendimentoClientePesquisa) {
        var propObj = _atendimentoClientePesquisa[prop];
        var propId = "#" + propObj.id;

        $(propId).keyup(delay(function (e) {
            pesquisar();
        }, 700));
    }
}

function delay(fn, ms) {
    var timer = 0
    return function (...args) {
        clearTimeout(timer)
        timer = setTimeout(fn.bind(this, ...args), ms || 0)
    }
}

function pesquisar(page, paginou) {
    var itensPorPagina = 10;

    var data = RetornarObjetoPesquisa(_atendimentoClientePesquisa);

    data.inicio = itensPorPagina * (page - 1);
    data.limite = itensPorPagina;

    executarReST("AtendimentoPedidoCliente/Pesquisa", data, function (arg) {
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
            $("#divPaginationAtendimentoPedidoCliente").html('<ul style="float:right" id="paginacaoAtendimentoPedidoCliente" class="pagination"></ul>');
            var paginas = Math.ceil((retorno.QuantidadeRegistros / itensPorPagina));
            $('#paginacaoAtendimentoPedidoCliente').twbsPagination({
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
            $("#divPaginationAtendimentoPedidoCliente").html('<span>Nenhum Registro Encontrado</span>');
    }
}
