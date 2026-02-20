/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */
var _chatAtendimentoPedido;

var _ListaMensagem = new Array();

var ChatAtendimentoPedido = function () {

    this.Pedido = PropertyEntity({ val: ko.observable("") });

    this.Mensagem = PropertyEntity({ text: ko.observable("Mensagem"), val: ko.observable(""), def: "", visible: ko.observable(true), maxlength: 500 });
    this.EnviarMensagem = PropertyEntity({ eventClick: enviarMensagemClick, type: types.event, text: "Enviar Mensagem", visible: ko.observable(true) });

};

/*
 * Declaração das Funções de Inicialização
 */
function processarMensagemChatEnviadaEvent(content) {
    mensagem = JSON.parse(content);
    for (var i = 0; i < _atendimentoClientePedido.Pedidos.val().length; i++) {
        if (_atendimentoClientePedido.Pedidos.val()[i].Codigo == mensagem.pedido) {
            $("#" + mensagem.pedido + " img").addClass("blink");
        }
    }

    if (_chatAtendimentoPedido != null && mensagem.pedido == _chatAtendimentoPedido.Pedido.val()) {
        adicionarMensagemChat(mensagem, false);
        scroolTextArea();
        if (_IDUsuario != mensagem.codigoRemetente)
            confirmarLeituraMensagem(mensagem.codigo);
    }
}

function processarMensagemRecebidaEvent(content) {
    mensagem = JSON.parse(content);
    if (_chatAtendimentoPedido != null && mensagem.pedido == _chatAtendimentoPedido.Pedido.val()) {
        $("#check_mensagem_recebida" + mensagem.codigo).show();
    }
}

function exibirModalMobileChatControleEntrega() {
    Global.abrirModal('divModalMobileChatControleEntrega');
    $("#divModalMobileChatControleEntrega").one('hidden.bs.modal', function () {
        LimparCampos(_chatAtendimentoPedido);
        $('#' + _chatAtendimentoPedido.Mensagem.id).html("");
        _ListaMensagem = new Array();
        _ListaPromotorNoChat = new Array();
    });
}

function adicionarHistorico(mensagem) {

    var html = "";
    var proprio = false;

    if (_IDUsuario == mensagem.codigoRemetente)
        proprio = true;

    html += '<div class="col" style="' + (proprio ? "float:right;" : "float:left;") + ' margin-bottom: 5px; max-width:70%; min-width:490px; padding-right:0px">';
    html += '<div class="well well-sm padding-5" style="background-color:' + (proprio ? "#475999" : "#FFFFF") + ';color:' + (proprio ? "#FFF" : "#666") + ';border-color:' + (proprio ? "#FFF" : "#666") + '">';
    html += '<small style="color:' + (proprio ? "#FFF" : "#666") + '"><b>' + mensagem.remetente + ':</b></small><p style="overflow:auto">';
    html += mensagem.mensagem;
    html += '<b style="float:right; font-size: 10px; margin-left:10px; margin-top:3px;">' + mensagem.dataMensagem;
    html += '<i id="check_mensagem_recebida' + mensagem.codigo + '" style="display: ' + (mensagem.visualizada && proprio ? "inline" : "none") + '; margin-left:2px;" class="fa fa-check"></i>';
    $('#' + _chatAtendimentoPedido.Mensagem.id).append(html);
}


function abrirChatAtendimento(pedido) {
    limparModalChat();
    Global.abrirModal("divModalChat");
    _chatAtendimentoPedido.Pedido.val(pedido.Codigo);
    $("#" + pedido.Codigo + " img").removeClass("blink");
    obterHistoricoChat();
}

function loadChatAtendimentoCliente() {
    _chatAtendimentoPedido = new ChatAtendimentoPedido();
    KoBindings(_chatAtendimentoPedido, "knoutChatAtendimentoPedido");
}

function obterHistoricoChat() {
    var data = { Pedido: _chatAtendimentoPedido.Pedido.val() };

    executarReST("AtendimentoPedidoCliente/ObterHistoricoMensagemChatAtendimentoPedido", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                for (var i = 0; i < arg.Data.length; i++) {
                    adicionarMensagemChat(arg.Data[i], true);
                }
                setTimeout(scroolTextArea, 200);
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
        }
    });
}


/*
 * Declaração das Funções Associadas a Eventos
 */

function scroolTextArea() {
    var objDiv = document.getElementById(_chatAtendimentoPedido.Mensagem.id);
    objDiv.scrollTop = objDiv.scrollHeight;
}

function adicionarMensagemChat(mensagem, addtodas) {
    var adicionar = true;
    if (!addtodas) {
        for (var i = 0; i < _ListaMensagem.length; i++) {
            if (_ListaMensagem[i].codigo == mensagem.codigo) {
                adicionar = false;
                break;
            }
        }
    }

    if (adicionar) {
        _ListaMensagem.push(mensagem);
        adicionarHistorico(mensagem);
    }
}

function enviarMensagemClick(e, sender) {
    if (_chatAtendimentoPedido.Mensagem.val() != "") {
        var mensagem = { Pedido: _chatAtendimentoPedido.Pedido.val(), Mensagem: _chatAtendimentoPedido.Mensagem.val() }

        executarReST("AtendimentoPedidoCliente/EnviarMensagemChat", mensagem, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    adicionarMensagemChat(arg.Data, false);
                    _chatAtendimentoPedido.Mensagem.val("");
                    scroolTextArea();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
        });
    }
}



function limparModalChat() {
    $('#' + _chatAtendimentoPedido.Mensagem.id).html("");
}