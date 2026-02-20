/*
 * Declaração de Objetos Globais do Arquivo
 */
var _mobileChatChamado;
var _chamadoOcorrenciaModalMobileChatChamado;

var _ListaMensagem = new Array();

var MobileChat = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.NomeMotorista = PropertyEntity({ val: ko.observable(''), def: '' });
    this.NumeroMotorista = PropertyEntity({ val: ko.observable(''), def: '' });

    this.Mensagem = PropertyEntity({ text: ko.observable("Mensagem:"), val: ko.observable(""), def: "", visible: ko.observable(true), maxlength: 500 });
    this.EnviarMensagem = PropertyEntity({ eventClick: enviarMensagemClick, type: types.event, text: "Enviar Mensagem", visible: ko.observable(true) });
    this.NaoLido = PropertyEntity({ eventClick: MarcarComoNaoLidoClick, type: types.event, text: "Marcar como não lido", visible: ko.observable(true) });
    this.AbrirWhatsApp = PropertyEntity({ type: types.event, text: "Abrir no WhatsApp", href: ko.computed(LinkAberturaWhatsApp.bind(this)), visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarChatModalClick, type: types.event, text: "Fechar Chat", visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function LinkAberturaWhatsApp() {
    var numeroTelefone = this.NumeroMotorista.val();
    var nomeMotorista = this.NomeMotorista.val();

    return 'https://api.whatsapp.com/send?phone=' + numeroTelefone + '&text=Ol%C3%A1%20' + nomeMotorista;
}


function marcarComoLido(carga) {
    //mudar botao
    if (_analise != null) {
        _analise.ChatMotorista.cssClass("btn btn-default");
    }
}

function marcarComoNaoLido(carga) {
    //mudar botao
    
    if (_analise != null && carga == _mobileChatChamado.Carga.val()) {
        _analise.ChatMotorista.cssClass("btn btn-default butonAlert");
    }
}

function processarMensagemChatEnviadaEvent(content) {
    mensagem = JSON.parse(content);
    if (_mobileChatChamado != null && mensagem.carga == _mobileChatChamado.Carga.val()) {
        adicionarMensagemChat(mensagem, false);
        scroolTextArea();
        if (_IDUsuario != mensagem.codigoRemetente)
            confirmarLeituraMensagem(mensagem.codigo);
    } else {
        if (_IDUsuario != mensagem.codigoRemetente)
            marcarComoNaoLido(mensagem.carga);
    }
}


function processarMensagemRecebidaEvent(content) {
    mensagem = JSON.parse(content);
    if (_mobileChatChamado != null && mensagem.carga == _mobileChatChamado.Carga.val()) {
        $("#check_mensagem_recebida" + mensagem.codigo).show();
    }
}

function exibirModalMobileChatChamado() {
    _chamadoOcorrenciaModalMobileChatChamado.show();
    $("#divModalMobileChatChamado").one('hidden.bs.modal', function () {
        LimparCampos(_mobileChatChamado);
        $('#' + _mobileChatChamado.Mensagem.id).html("");
        _ListaMensagem = new Array();
    });
}

function adicionarHistorico(mensagem) {

    var html = "";
    var proprio = false;

    if (_IDUsuario == mensagem.codigoRemetente)
        proprio = true;

    html += '<div class="mw-75 d-flex ' + (proprio ? "justify-content-end" : "justify-content-start") + ' m-2">'
    html += '<div class="col-12 col-md-auto mb-2" >';
    html += '<div class="card" style="background-color:' + (proprio ? "#f6ffef" : "aliceblue") + '">';
    html += '<div class="card-body">';
    html += '<div class="row">';
    html += '<span class="mb-3"><b class="txt-color-blue">' + mensagem.remetente + ':</b></span>';
    html += '<div class="d-flex justify-content-between">';
    html += '<p style: display:"inline-block; overflow: hidden;">' + mensagem.mensagem + '</p>';
    html += '<b style="font-size: 10px; margin-left:10px; margin-top:3px;">' + mensagem.dataMensagem;
    html += '</div>';
    html += '<i id="check_mensagem_recebida' + mensagem.codigo + '" style="display: ' + (mensagem.visualizada && proprio ? "inline" : "none") + '; margin-left:2px;" class="fal fa-check"></i></b></div></div>';
    html += '</div>';
    html += '</div>';
    html += '</div>';
    $('#' + _mobileChatChamado.Mensagem.id).append(html);
}

function loadMobileChatChamados(dados) {
    _mobileChatChamado = new MobileChat();
    KoBindings(_mobileChatChamado, "knockouMobileChatModalChamado");

    _mobileChatChamado.Carga.val(dados.Carga);

    if (_CONFIGURACAO_TMS.PermitirContatoWhatsApp && dados.NumeroMotorista != '') {
        _mobileChatChamado.NomeMotorista.val(dados.NomeMotorista);
        _mobileChatChamado.NumeroMotorista.val(dados.NumeroMotorista);
        _mobileChatChamado.AbrirWhatsApp.visible(true);
    } else {
        _mobileChatChamado.AbrirWhatsApp.visible(false);
    }

    $(document).unbind('keypress');
    $(document).bind('keypress', function (e) {
        if (e.keyCode == 13) {
            $('#' + _mobileChatChamado.EnviarMensagem.id).trigger('click');
        }
    });


    obterHistoricoChat();

    _chamadoOcorrenciaModalMobileChatChamado = new bootstrap.Modal(document.getElementById("divModalMobileChatChamado"), { backdrop: 'static' });
}

function obterHistoricoChat() {
    var data = { Carga: _mobileChatChamado.Carga.val() };

    executarReST("ControleEntrega/ObterHistoricoMensagemChatMobile", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                for (var i = 0; i < arg.Data.length; i++) {
                    adicionarMensagemChat(arg.Data[i], true);
                }
                marcarComoLido(_mobileChatChamado.Carga.val());
                exibirModalMobileChatChamado();
                setTimeout(scroolTextArea, 200);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    });
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function scroolTextArea() {
    var objDiv = document.getElementById(_mobileChatChamado.Mensagem.id);
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

function MarcarComoNaoLidoClick(e, sender) {
    var mensagem = { Carga: _mobileChatChamado.Carga.val() }
    executarReST("ControleEntrega/MarcarComoNaoLido", mensagem, function (arg) {
        if (arg.Success) {
            marcarComoNaoLido(_mobileChatChamado.Carga.val());
            limparModalChat();
        }
        else
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
    });
}


function enviarMensagemClick(e, sender) {
    if (_mobileChatChamado.Mensagem.val() != "") {
        var mensagem = { Carga: _mobileChatChamado.Carga.val(), Mensagem: _mobileChatChamado.Mensagem.val() }
        executarReST("ControleEntrega/EnviarMensagemChat", mensagem, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    adicionarMensagemChat(arg.Data, false);
                    _mobileChatChamado.Mensagem.val("");
                    scroolTextArea();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        });
    }
}

function cancelarChatModalClick() {
    limparModalChat();
}

function limparModalChat() {
    $('#' + _mobileChatChamado.Mensagem.id).html("");
    LimparCampos(_mobileChatChamado);
    _chamadoOcorrenciaModalMobileChatChamado.hide();
}