/// <reference path="../../Enumeradores/EnumSituacaoSeparacaoPedido.js" />
/// <reference path="SeparacaoPedido.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _HTMLDetalheSeparacaoPedido;
var _AreaPedido;
var _detalhePedido;
var _pedidosPai = [];

var _knoutsPedidos = new Array();

var AreaPedido = function () {
    this.Pedido = PropertyEntity();
    this.CarregandoPedidos = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.Inicio = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Total = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, eventChange: PedidosPesquisaScroll });
};

var DetalheSeparacaoPedido = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataCarregamentoPedido = PropertyEntity({ text: "Data: ", val: ko.observable("") });
    this.Filial = PropertyEntity({ text: "Filial: ", val: ko.observable("") });
    this.NumeroPedidoEmbarcador = PropertyEntity({ text: "Pedido Embarcador: ", val: ko.observable("") });
    this.Destino = PropertyEntity({ text: "Destino: ", val: ko.observable("") });
    this.Remetente = PropertyEntity({ text: "Remetente: ", val: ko.observable("") });
    this.Destinatario = PropertyEntity({ text: "Destinatário: ", val: ko.observable("") });
    this.Peso = PropertyEntity({ text: "Peso: ", val: ko.observable("") });
    this.NumeroPedido = PropertyEntity({ text: "Número Pedido (sequencial): ", val: ko.observable("") });
    this.DisponivelParaSeparacao = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.string });
    this.Agrupamento = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool });
    this.Reentrega = PropertyEntity({ text: "Reentrega", val: ko.observable(false), getType: typesKnockout.bool });
    this.PrevisaoEntregaTeorica = PropertyEntity({ text: "Prev. Entrega Teórica:", val: ko.observable(""), getType: typesKnockout.string, visible: ko.observable(false) });
    this.InfoPedido = PropertyEntity({ eventClick: selecionarPedidoClick, type: types.event, cssClass: ko.observable("well well-pedido no-padding padding-5") });
};

function loadDetalhesPedido(callback) {
    _AreaPedido = new AreaPedido();
    KoBindings(_AreaPedido, "knoutAreaPedido");

    $.get("Content/Static/MontagemCarga/DetalhePedido.html?dyn=" + guid(), function (data) {
        _HTMLDetalheSeparacaoPedido = data;
        if (callback != null)
            callback();
    });
}

function selecionarPedidoClick(e) {
    if (_separacaoPedido.Situacao.val() === EnumSituacaoSeparacaoPedido.Aberto) {
        var pedido = RetornarObjetoPesquisa(e);
        var index = obterIndicePedido(pedido);
        if (index >= 0) {
            e.InfoPedido.cssClass("well well-pedido no-padding padding-5");
            _separacaoPedido.Pedidos.val().splice(index, 1);
            RemoverPedido(pedido);
            if (pedido.Agrupamento != "") 
                removerPedidosMesmoAgrupamento(pedido);
            removerPedidoPai(pedido);
        } else {
            adicionarPedidoCarregamento(pedido, e, false);
            if (pedido.Agrupamento != "") {
                selecionarPedidosMesmoAgrupamento(pedido);
                _pedidosPai.push(pedido);
            }
        }
    }
}

function selecionarPedidosMesmoAgrupamento(pedido) {
    for (var i = 0; i < _knoutsPedidos.length; i++) {
        var pedidoNaLista = _knoutsPedidos[i];
        var pedidoObjeto = RetornarObjetoPesquisa(pedidoNaLista);
        var index = obterIndicePedido(pedidoObjeto);
        if (pedidoNaLista.Agrupamento.val() == pedido.Agrupamento && pedidoNaLista.DisponivelParaSeparacao.val() && pedidoNaLista.Codigo.val() != pedido.Codigo && index == -1) {
            var pedido = RetornarObjetoPesquisa(pedidoNaLista);
            adicionarPedidoCarregamento(pedido, pedidoNaLista, false);
        }
    }
}

function removerPedidosMesmoAgrupamento(pedido) {
    for (var i = 0; i < _knoutsPedidos.length; i++) {
        var pedidoNaLista = _knoutsPedidos[i];
        var pedidoObjeto = RetornarObjetoPesquisa(pedidoNaLista);
        var index = obterIndicePedido(pedidoObjeto);
        if (pedidoNaLista.Agrupamento.val() == pedido.Agrupamento && pedidoNaLista.DisponivelParaSeparacao.val() && pedidoNaLista.Codigo.val() != pedido.Codigo && index != -1) {
            pedidoNaLista.InfoPedido.cssClass("well well-pedido no-padding padding-5");
            pedidoNaLista.InfoPedido.eventClick = selecionarPedidoClick;
            _separacaoPedido.Pedidos.val().splice(index, 1);
        }
    }
}

function removerPedidoPai(pedido){
    for (var i = 0; i < _pedidosPai.length; i++) {
        if (pedido.Agrupamento == _pedidosPai[i].Agrupamento)
            _pedidosPai.splice(i, 1);
    }
}

function adicionarPedidoCarregamento(pedido, knoutPedido, primeiraPedido) {
    knoutPedido.InfoPedido.cssClass("well well-pedido-selecionada no-padding padding-5");
    _separacaoPedido.Pedidos.val().push(pedido);

    if (primeiraPedido)
        VerificarCompatibilidasKnoutsPedido();
}

function buscarPedidosMontagem() {
    var data = RetornarObjetoPesquisa(_pesquisaSeparacaoPedido);
    data.Inicio = _AreaPedido.Inicio.val();
    data.Limite = 20;
    _ControlarManualmenteProgresse = true;
    _AreaPedido.CarregandoPedidos.val(true);
    executarReST("SeparacaoPedido/ObterPedidos", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                var retorno = arg.Data;
                _AreaPedido.Total.val(retorno.Quantidade);
                _AreaPedido.Inicio.val(_AreaPedido.Inicio.val() + data.Limite);

                for (var i = 0; i < retorno.Registros.length; i++) {
                    var pedido = retorno.Registros[i];
                    var knoutDetalhePedido = new DetalheSeparacaoPedido();

                    knoutDetalhePedido.PrevisaoEntregaTeorica.visible(_CONFIGURACAO_TMS.PermitirInformarLeadTimeTabelaFreteCliente && !string.IsNullOrWhiteSpace(pedido.PrevisaoEntregaTeorica));

                    var html = _HTMLDetalheSeparacaoPedido.replace(/#detalhePedido/g, knoutDetalhePedido.InfoPedido.id);
                    $("#" + _AreaPedido.Pedido.id).append(html);
                    KoBindings(knoutDetalhePedido, knoutDetalhePedido.InfoPedido.id);
                    var dataKnout = { Data: pedido };
                    PreencherObjetoKnout(knoutDetalhePedido, dataKnout);
                    _knoutsPedidos.push(knoutDetalhePedido);

                    index = obterIndicePedido(pedido);
                    if (index >= 0)
                        knoutDetalhePedido.InfoPedido.cssClass("well well-pedido-selecionada no-padding padding-5");
                    else
                        pedidoSelecionavel(knoutDetalhePedido);
                }
                for (var i = 0; i < _pedidosPai.length; i++) {
                    selecionarPedidosMesmoAgrupamento(_pedidosPai[i]);
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, "Falha", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
        _ControlarManualmenteProgresse = false;
        _AreaPedido.CarregandoPedidos.val(false);
    });
}

function PedidosPesquisaScroll(e, sender) {
    var elem = sender.target;
    if (_AreaPedido.Inicio.val() < _AreaPedido.Total.val() &&
        elem.scrollTop >= (elem.scrollHeight - elem.offsetHeight - 5)) {
        buscarPedidosMontagem();
    }
}

function RemoverPedido(pedido) {
    if (_separacaoPedido.Codigo.val() <= 0 && _separacaoPedido.Pedidos.val().length === 0) {
        LimparCampos(_separacaoPedido);
        desmarcarKnoutsPedido();
    }
}

function obterIndicePedido(pedido) {
    if (!NavegadorIEInferiorVersao12()) {
        return _separacaoPedido.Pedidos.val().findIndex(function (item) { return item.Codigo === pedido.Codigo; });
    } else {
        for (var i = 0; i < _separacaoPedido.Pedidos.val().length; i++) {
            if (pedido.Codigo === _separacaoPedido.Pedidos.val()[i].Codigo)
                return i;
        }
        return -1;
    }
}

function obterIndiceKnoutPedido(pedido) {
    if (!NavegadorIEInferiorVersao12()) {
        return _knoutsPedidos.findIndex(function (item) { return item.Codigo.val() === pedido.Codigo; });
    } else {
        for (var i = 0; i < _knoutsPedidos.length; i++) {
            if (pedido.Codigo === _knoutsPedidos[i].Codigo.val())
                return i;
        }
        return -1;
    }
}

function desmarcarKnoutsPedido() {
    for (var i = 0; i < _knoutsPedidos.length; i++) {
        var knoutPedido = _knoutsPedidos[i];
        knoutPedido.InfoPedido.cssClass("well well-pedido no-padding padding-5");
        knoutPedido.InfoPedido.eventClick = selecionarPedidoClick;
    }
}


function VerificarCompatibilidasKnoutsPedido() {
    for (var i = 0; i < _knoutsPedidos.length; i++) {
        var knoutPedido = _knoutsPedidos[i];
        pedidoSelecionavel(knoutPedido);
    }
}

function pedidoSelecionavel(knoutPedido) {
    return true;
}