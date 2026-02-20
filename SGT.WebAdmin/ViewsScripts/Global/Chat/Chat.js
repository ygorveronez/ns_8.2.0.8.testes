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
/// <reference path="../../../js/libs/jquery.signalR-2.2.0.js" />
/// <reference path="../../../js/smart-chat-ui/smart.chat.manager.js" />
/// <reference path="../../../js/smart-chat-ui/smart.chat.ui.js" />
/// <reference path="../SignalR/SignalR.js" />
/// <reference path="SignalRChat.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _chatGlobal;
var filter_input, chat_users_container, chat_users, chat_list_btn, chat_body;
var carregandoMensagens = false;
var estaUtilizandoChat = false;
//para o chat
var boxList = [],
    showList = [],
    nameList = [],
    idList = [],
    chatbox_config = {
        width: 200,
        gap: 35
    },
    ignore_key_elms = ["#header, #left-panel, #right-panel, #main, div.page-footer, #shortcut, #divSmallBoxes, #divMiniIcons, #divbigBoxes, #voiceModal, script, .ui-chatbox, .ui-autocomplete"];

var ChatGlobal = function () {
    this.ListaUsuarios = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.ListaConversaUsuario = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
};

//*******EVENTOS*******

function loadChatGlobal(callback) {

    _chatGlobal = new ChatGlobal();
    KoBindings(_chatGlobal, "knoutChatGlobal");

    //filter_input = $('#filter-chat-list');
    //chat_users_container = $('#chat-container > .chat-list-body');
    //chat_users = $('#chat-users');
    //chat_list_btn = $('#chat-container > .chat-list-open-close');
    //chat_body = $('#chat-body');
    $('#chat-container').hide();

    if (_CONFIGURACAO_TMS.UtilizaChat) {
        //chat_list_btn.show();
        //listFilterChat();
        //listOpenChat();
        //buscarUsuariosEmpresa();
        //estaUtilizandoChat = true;
        $('#chat-container').show();
    }
    //LoadConexaoSignalRChat();
}

function buscarUsuariosEmpresa() {
    executarReST("Chat/BuscarUsuariosChat", null, function (r) {
        if (r.Success) {
            _chatGlobal.ListaUsuarios.list = r.Data;
            carregarListaUsuarios();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function carregarListaUsuarios() {

    var newlink = "";
    var listaConversasNaoLidas = [];

    $.each(_chatGlobal.ListaUsuarios.list, function (i, listaUsuarios) {
        var partLink = "";
        if (listaUsuarios.naoLidas > 0) {
            partLink += '  <span id="naoLidas' + listaUsuarios.CodigoUsuario + '" class="badge badge-inverse">' + listaUsuarios.naoLidas + '</span>';

            var listaNaoLida = new Object();//Preenche para acionar os chats de conversas não lidas
            listaNaoLida.idLink = listaUsuarios.idLink;
            listaNaoLida.idChat = listaUsuarios.idChat;
            listaConversasNaoLidas.push(listaNaoLida);
        }
        if (listaUsuarios.status == "online")
            partLink += '<span id="ativo' + listaUsuarios.CodigoUsuario + '" class="state"><i class="fa fa-circle txt-color-green pull-right"></i></span>';

        newlink += '<li><a href="javascript:void(0);" class="d-table w-100 px-2 py-2 text-dark hover-white" data-idchat="' + listaUsuarios.idChat +
            '" data-first_name="' + listaUsuarios.first_name +
            '" data-last_name="' + listaUsuarios.last_name +
            '" data-status="' + listaUsuarios.status +
            '" data-alertmsg="' + listaUsuarios.alertmsg +
            '" data-alertshow="' + listaUsuarios.alertshow +
            '" data-cod_user="' + listaUsuarios.CodigoUsuario +
            '" id="' + listaUsuarios.idLink +
            '"><img src="img/avatars/male.png">' + listaUsuarios.first_name +
            partLink +
            '</a>';

        var listaDadosConversa = new Object();
        listaDadosConversa.CodigoUsuario = listaUsuarios.CodigoUsuario;
        listaDadosConversa.Inicio = 0;
        listaDadosConversa.Total = listaUsuarios.total;

        _chatGlobal.ListaConversaUsuario.list.push(listaDadosConversa);
    });

    chat_users.html(newlink);

    for (var i = 0; i < listaConversasNaoLidas.length; i++) {
        openChatClienteMessagemNaoLida(listaConversasNaoLidas[i].idLink, listaConversasNaoLidas[i].idChat);
    }
}


function listFilterChat() {
    // custom css expression for a case-insensitive contains()
    jQuery.expr[':'].Contains = function (a, i, m) {
        return (a.textContent || a.innerText || "").toUpperCase().indexOf(m[3].toUpperCase()) >= 0;
    };

    function listFilter(list) {
        // header is any element, list is an unordered list
        // create and add the filter form to the header

        filter_input.change(function () {
            var filter = $(this).val();
            if (filter) {
                // this finds all links in a list that contain the input,
                // and hide the ones not containing the input while showing the ones that do
                chat_users.find("a:not(:Contains(" + filter + "))").parent().slideUp();
                chat_users.find("a:Contains(" + filter + ")").parent().slideDown();
            } else {
                chat_users.find("li").slideDown();
            }
            return false;
        }).keyup(function () {
            // fire the above change event after every letter
            $(this).change();
        });
    }

    // on dom ready
    listFilter(chat_users);

    // open chat list
    chat_list_btn.click(function () {
        if ($(this).parent('#chat-container').hasClass('open')) {
            $(this).parent('#chat-container').removeClass("open");
            document.getElementById("knoutChatGlobal").style.display = "none";
            //document.getElementById("chat-container").style.width = "0px";
            //document.getElementById("chat-container").style.right = "1px";
        } else {
            document.getElementById("knoutChatGlobal").style.display = "block";
            //document.getElementById("chat-container").style.width = null;
            //document.getElementById("chat-container").style.right = null;
            $(this).parent('#chat-container').addClass("open");
        }
    });
}

function listOpenChat() {

    chat_users.on("click", "a", function (event, ui) {
        $this = $(this);
        var idChat = $this.data("idchat");
        var codUser = $this.data("cod_user")

        chatbox_config.width = 230;
        chatboxManager.addBox(idChat,
            {
                cod_user: codUser,
                first_name: $this.data("first_name"),
                last_name: $this.data("last_name"),
                status: $this.data("status"),
                alertmsg: $this.data("alertmsg"),
                alertshow: $this.data("alertshow"),
                messageSent: function (id, user, msg) {
                    //$("#" + id).chatbox("option", "boxManager").addMsg("Eu - " + Global.DataHoraSegundoAtual(), "\n" + msg); //Vai mostrar na tela apenas após salvar, assim irá receber em todos os chats de envio
                    saveMessage(user.cod_user, msg);
                },
                boxClosed: function (id) {
                } // called when the close icon is clicked
            });
        event.preventDefault();

        var inicioLoad = 0;
        for (var i = 0; i < _chatGlobal.ListaConversaUsuario.list.length; i++) {
            if (_chatGlobal.ListaConversaUsuario.list[i].CodigoUsuario == codUser) {
                inicioLoad = _chatGlobal.ListaConversaUsuario.list[i].Inicio;
                break;
            }
        }
        if (inicioLoad == 0)
            loadMessages(idChat, codUser);

        marcarMensagensComoLida(codUser);
        $('span[id^="naoLidas' + codUser + '"]').remove(); //Deleta o não lida do chat

        $("#" + idChat).on("scroll", function (e) {
            if (!carregandoMensagens) {
                var elem = e.currentTarget;
                if (elem.scrollTop < 50) {
                    loadMessages(idChat, codUser, elem);
                }
            }
        });
    });
}

function saveMessage(cod_user, msg) {
    var data = { cod_user: cod_user, msg: msg };
    executarReST("Chat/SaveMessage", data, function (r) {
        if (!r.Success) {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    }, null, false);
}

function loadMessages(id, cod_user, e) {
    carregandoMensagens = true;
    var quantidadePorVez = 10;
    var inicio = 0;
    var total = 0;
    for (var i = 0; i < _chatGlobal.ListaConversaUsuario.list.length; i++) {
        if (_chatGlobal.ListaConversaUsuario.list[i].CodigoUsuario == cod_user) {
            inicio = _chatGlobal.ListaConversaUsuario.list[i].Inicio;
            total = _chatGlobal.ListaConversaUsuario.list[i].Total;
            break;
        }
    }

    if (inicio < total) {
        var data = { cod_user: cod_user, inicio: inicio, limite: quantidadePorVez };
        executarReST("Chat/LoadMessages", data, function (r) {
            if (r.Success) {
                //var myNode = document.getElementById(id);//Deleta todas as mensagens da conversa antes de carregar caso tenha
                //while (myNode.firstChild) {
                //    myNode.removeChild(myNode.firstChild);
                //}
                var alturaOriginal = 0;
                if (inicio != 0)
                    alturaOriginal = e.scrollHeight;

                _chatGlobal.ListaConversaUsuario.list[i].Inicio = inicio + quantidadePorVez;
                $.each(r.Data.ListaMensagens, function (i, listaMensagens) {
                    if (inicio == 0)
                        $("#" + id).chatbox("option", "boxManager").addMsg(listaMensagens.QuemEnviou + " - " + listaMensagens.DataEnvio, "\n" + listaMensagens.Mensagem);
                    else {
                        $("#" + id).prepend('<div class="ui-chatbox-msg" style="display: block; max-width: 228px;"><b>' +
                            listaMensagens.QuemEnviou + " - " + listaMensagens.DataEnvio +
                            ': </b><span>' + "\n" + listaMensagens.Mensagem + '</span></div>');
                    }
                });
                carregandoMensagens = false;

                if (inicio != 0) {
                    var altura = e.scrollHeight;
                    var diferenca = altura - alturaOriginal;
                    e.scrollTop = diferenca;
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        }, null, false);
    }
}

function openChatClienteSignalR(idLink, idChat, msg, dataHoraEnvio, quemEnviou) {
    $('#' + idLink).trigger('click');
    var chatId = $('#' + idChat);
    chatId.append('<div class="ui-chatbox-msg" style="display: block; max-width: 228px;"><b>' +
        quemEnviou + " - " + dataHoraEnvio +
        ': </b><span>' + "\n" + msg + '</span></div>');
    chatId.trigger('click');
    chatId.trigger('click');
}

function openChatClienteMessagemNaoLida(idLink, idChat) {
    $('#' + idLink).trigger('click');
    var chatId = $('#' + idChat);
    chatId.trigger('click');
    chatId.trigger('click');
}

function atualizaStatusUsuarioSignalR(codigoUsuario, conectado) {
    if (estaUtilizandoChat) { //Valida, pois o SignalR do chat sempre inicia mesmo não configurado
        var chatId = $("#chat" + codigoUsuario);
        var chatBox = $("#box_" + codigoUsuario).parent().siblings('.ui-widget-header');

        if (!conectado) {
            $('span[id^="ativo' + codigoUsuario + '"]').remove();
            chatId.data('status', 'incognito');
            chatBox.removeClass('online').addClass('incognito');
        }
        else {
            chatId.append('<span id="ativo' + codigoUsuario + '" class="state"><i class="fa fa-circle txt-color-green pull-right"></i></span>');
            chatId.data('status', 'online');
            chatBox.removeClass('incognito').addClass('online');
        }
    }
}

function marcarMensagensComoLida(cod_user) {
    var data = { cod_user: cod_user };
    executarReST("Chat/MarcarMensagensComoLida", data, function (r) {
        if (!r.Success) {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    }, null, false);
}