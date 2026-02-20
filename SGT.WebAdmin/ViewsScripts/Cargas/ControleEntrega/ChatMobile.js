/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */
var _mobileChatControleEntrega;
var _mobileChatAdicionarPromotor;
var _mobileChatRemoverPromotor;
var _gridPedidosAdicionarPromotor = null;
//var _gridPromotoresChat = null;
//var _message_id = 1;
//var _socket = null;
//var _subscribed_room = [];

var _ListaMensagem = new Array();
var _ListaPromotorNoChat = new Array();

var MobileChat = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.NomeMotorista = PropertyEntity({ val: ko.observable(''), def: '' });
    this.NumeroMotorista = PropertyEntity({ val: ko.observable(''), def: '' });

    this.Mensagem = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.ControleEntrega.Mensagem.getFieldDescription()), val: ko.observable(""), def: "", visible: ko.observable(true), maxlength: 500 });
    this.AddPromotor = PropertyEntity({ eventClick: addPromotorMensagemClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.AdicionarPromotor, visible: ko.observable(false) });
    this.RemoverPromotor = PropertyEntity({ eventClick: RemoverPromotorMensagemClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.RemoverPromotor, visible: ko.observable(false) });
    this.EnviarMensagem = PropertyEntity({ eventClick: enviarMensagemClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.EnviarMensagem, visible: ko.observable(true) });
    this.NaoLido = PropertyEntity({ eventClick: MarcarComoNaoLidoClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.MarcarComoNaoLido, visible: ko.observable(true) });
    this.AbrirWhatsApp = PropertyEntity({ type: types.event, text: Localization.Resources.Cargas.ControleEntrega.AbrirNoWhatsApp, href: ko.computed(LinkAberturaWhatsApp.bind(this)), visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarChatModalClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.FecharChat, visible: ko.observable(true) });
}


var AdicionarPromotorChat = function () {
    this.GridPedidosPromotor = PropertyEntity({});
}

var RemoverPromotorChat = function () {
    this.GridPromotoresChat = PropertyEntity({});
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
    for (var i = 0; i < _KnoutsEntregas.length; i++) {
        if (_KnoutsEntregas[i].Carga.val() == carga) {
            _KnoutsEntregas[i].ImagemMensagem.val("../../../../Content/TorreControle/Icones/gerais/mensagem.svg");
        }
    }
}

function marcarComoNaoLido(carga) {
    for (var i = 0; i < _KnoutsEntregas.length; i++) {
        if (_KnoutsEntregas[i].Carga.val() == carga) {
            _KnoutsEntregas[i].ImagemMensagem.val("../../../../Content/TorreControle/Icones/gerais/mensagem-nao-lida.svg");
        }
    }
}

function processarMensagemChatEnviadaEvent(content) {
    mensagem = JSON.parse(content);
    if (_mobileChatControleEntrega != null && mensagem.carga == _mobileChatControleEntrega.Carga.val()) {
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
    if (_mobileChatControleEntrega != null && mensagem.carga == _mobileChatControleEntrega.Carga.val()) {
        $("#check_mensagem_recebida" + mensagem.codigo).show();
    }
}

function exibirModalMobileChatControleEntrega() {
    Global.abrirModal("divModalMobileChatControleEntrega");
    $("#divModalMobileChatControleEntrega").one('hidden.bs.modal', function () {
        LimparCampos(_mobileChatControleEntrega);
        $('#' + _mobileChatControleEntrega.Mensagem.id).html("");
        _ListaMensagem = new Array();
        _ListaPromotorNoChat = new Array();
    });
}

function adicionarHistorico(mensagem) {

    var html = "";
    var proprio = false;

    if (_IDUsuario == mensagem.codigoRemetente)
        proprio = true;

    html += '<div class="col mb-2" style="' + (proprio ? "float:right;" : "") + ' min-width:100%;">';
    html += '<div class="card txt-color-darken p-3" style="background-color:' + (proprio ? "#f6ffef" : "aliceblue") + '">';
    html += '<div class="card-body">';
    html += '<div class="row">';
    html += '<small class="txt-color-blue"><b>' + mensagem.remetente + ':</b></small><p style="overflow:auto">';
    html += mensagem.mensagem;
    html += '<b style="float:right; font-size: 10px; margin-left:10px; margin-top:3px;">' + mensagem.dataMensagem;
    html += '<i id="check_mensagem_recebida' + mensagem.codigo + '" style="display: ' + (mensagem.visualizada && proprio ? "inline" : "none") + '; margin-left:2px;" class="fal fa-check"></i>';
    html += '<i id="check_falha_mensagem_integracao' + mensagem.codigo + '" style="display: ' + (mensagem.falhaIntegracao && proprio ? "inline" : "none") + '; margin-left:2px; cursor: pointer;" title="Falha ao integrar mensagem" class="fal fa-exclamation-triangle" onclick="reenviarMensagemIntegracao(' + mensagem.codigo + ')""></i></b ></p ></div ></div > <div class="clearfix"></div>';
    html += '</div>';
    html += '</div>';
    $('#' + _mobileChatControleEntrega.Mensagem.id).append(html);
}

function loadMobileChatControleEntrega(dados) {
    _mobileChatControleEntrega = new MobileChat();
    KoBindings(_mobileChatControleEntrega, "knockouMobileChatModalControleEntrega");

    _mobileChatAdicionarPromotor = new AdicionarPromotorChat();
    KoBindings(_mobileChatAdicionarPromotor, "knockouMobileChatAdicionarPromotor");

    _mobileChatRemoverPromotor = new RemoverPromotorChat();
    KoBindings(_mobileChatRemoverPromotor, "knockouMobileChatRemoverPromotor");

    _mobileChatControleEntrega.Carga.val(dados.Carga);
    if (_CONFIGURACAO_TMS.PermitirContatoWhatsApp && dados.NumeroMotorista != '') {
        _mobileChatControleEntrega.NomeMotorista.val(dados.NomeMotorista);
        _mobileChatControleEntrega.NumeroMotorista.val(dados.NumeroMotorista);
        _mobileChatControleEntrega.AbrirWhatsApp.visible(true);
    } else {
        _mobileChatControleEntrega.AbrirWhatsApp.visible(false);
    }

    if (dados.AddPromotor) {
        _mobileChatControleEntrega.AddPromotor.visible(true);
        obterListaPromotorAdicionadoChat();
    }

    $(document).unbind('keypress');
    $(document).bind('keypress', function (e) {
        if (e.keyCode == 13) {
            $('#' + _mobileChatControleEntrega.EnviarMensagem.id).trigger('click');
        }
    });


    obterHistoricoChat();
}

function obterHistoricoChat() {
    var data = { Carga: _mobileChatControleEntrega.Carga.val() };

    executarReST("ControleEntrega/ObterHistoricoMensagemChatMobile", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                for (var i = 0; i < arg.Data.length; i++) {
                    adicionarMensagemChat(arg.Data[i], true);
                }
                marcarComoLido(_mobileChatControleEntrega.Carga.val());
                exibirModalMobileChatControleEntrega();
                setTimeout(scroolTextArea, 200);
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
        }
    });
}

function reenviarMensagemIntegracao(codigoMensagem) {
    var data = { Codigo: codigoMensagem };

    executarReST("ControleEntrega/reenviarMensagemIntegracao", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (arg.Data.Codigo) {
                    $("#check_falha_mensagem_integracao" + arg.Data.Codigo).hide();
                    $("#check_mensagem_recebida" + arg.Data.Codigo).show();
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
        }
    });

}

function obterListaPromotorAdicionadoChat() {
    var data = { Carga: _mobileChatControleEntrega.Carga.val() };

    executarReST("ControleEntrega/ObterListaPromotorAdicionadoAoChat", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                for (var i = 0; i < arg.Data.length; i++) {
                    _ListaPromotorNoChat.push({ NomeVendedor: arg.Data[i].Vendedor, CodigoVendedor: arg.Data[i].CodigoVendedor, NotaFiscal: arg.Data[i].CodigoNota });
                    _mobileChatControleEntrega.RemoverPromotor.visible(true);
                }
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
    var objDiv = document.getElementById(_mobileChatControleEntrega.Mensagem.id);
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
    if (_mobileChatControleEntrega.Mensagem.val() != "") {
        var mensagem = { Carga: _mobileChatControleEntrega.Carga.val(), Mensagem: _mobileChatControleEntrega.Mensagem.val(), ListaPromotorEnviar: JSON.stringify(_ListaPromotorNoChat) }
        executarReST("ControleEntrega/EnviarMensagemChat", mensagem, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    adicionarMensagemChat(arg.Data, false);
                    _mobileChatControleEntrega.Mensagem.val("");
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

function addPromotorMensagemClick(e, sender) {
    inicializaGridPedidosAdicionarPromotor();
    _gridPedidosAdicionarPromotor.CarregarGrid();

    Global.abrirModal("divModalMobileChatAdicionarPromotor");
}


function RemoverPromotorMensagemClick(e, sender) {
    inicializaGridRemoverPromotor();
    _gridPromotoresChat.CarregarGrid();

    Global.abrirModal("divModalMobileChatRemoverPromotor");
}

function inicializaGridPedidosAdicionarPromotor() {

    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = [
        { descricao: Localization.Resources.Gerais.Geral.Selecionar, id: guid(), evento: "onclick", metodo: onClickSelecionarPromotor, tamanho: "20", icone: "" },
    ];

    _gridPedidosAdicionarPromotor = new GridView(_mobileChatAdicionarPromotor.GridPedidosPromotor.id, "ControleEntrega/ObterNotasChatAdicionarPromotor", {
        Codigo: _mobileChatControleEntrega.Carga
    }, menuOpcoes);
}

function onClickSelecionarPromotor(promotor) {
    var adicionar = true;
    for (var i = 0; i < _ListaPromotorNoChat.length; i++) {
        if (_ListaPromotorNoChat[i].CodigoVendedor == promotor.CodigoVendedor) {
            adicionar = false;
            break;
        }
    }

    if (adicionar) {
        _ListaPromotorNoChat.push({ NomeVendedor: promotor.Vendedor, CodigoVendedor: promotor.CodigoVendedor, NotaFiscal: promotor.CodigoNota });
        enviarMensagemPromotorAdicionado(promotor);
        _mobileChatControleEntrega.RemoverPromotor.visible(true);
    } else {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sucesso.Atencao, "Promotor já adicionado ao chat");
    }

    Global.fecharModal("divModalMobileChatAdicionarPromotor");
}


function inicializaGridRemoverPromotor() {

    let menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = [
        { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), evento: "onclick", metodo: onClickRemoverPromotor, tamanho: "20", icone: "" },
    ];

    _gridPromotoresChat = new GridView(_mobileChatRemoverPromotor.GridPromotoresChat.id, "ControleEntrega/ConsultaPromotorAdicionadoAoChat", {
        Codigo: _mobileChatControleEntrega.Carga
    }, menuOpcoes);
}

function onClickRemoverPromotor(promotor) {
    var promotorRemover = null;
    for (var i = 0; i < _ListaPromotorNoChat.length; i++) {
        if (_ListaPromotorNoChat[i].CodigoVendedor == promotor.CodigoVendedor) {
            promotorRemover = promotor;
            _ListaPromotorNoChat.splice(i, 1)
            break;
        }
    }

    if (promotorRemover != null) {
        var dados = { Carga: _mobileChatControleEntrega.Carga.val(), Promotor: promotorRemover.CodigoVendedor }
        executarReST("ControleEntrega/RemoverPromotorChat", dados, function (arg) {
            if (arg.Success) {
                enviarMensagemPromotorRemovido(promotorRemover);
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
        });
    }

    if (_ListaPromotorNoChat.length <= 0)
        _mobileChatControleEntrega.RemoverPromotor.visible(false);

    Global.fecharModal("divModalMobileChatRemoverPromotor");
}

function enviarMensagemPromotorAdicionado(promotor) {
    var msg = "Promotor " + promotor.CodigoVendedor + " - " + promotor.Vendedor + " adicionado a conversa";

    var mensagem = { Carga: _mobileChatControleEntrega.Carga.val(), Mensagem: msg, ListaPromotorEnviar: JSON.stringify(_ListaPromotorNoChat) }
    executarReST("ControleEntrega/EnviarMensagemChat", mensagem, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                adicionarMensagemChat(arg.Data, false);
                _mobileChatControleEntrega.Mensagem.val("");
                scroolTextArea();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        }
        else
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
    });
}

function enviarMensagemPromotorRemovido(promotor) {
    var msg = "Promotor " + promotor.CodigoVendedor + " - " + promotor.Vendedor + " removido da conversa";

    var mensagem = { Carga: _mobileChatControleEntrega.Carga.val(), Mensagem: msg, ListaPromotorEnviar: JSON.stringify(_ListaPromotorNoChat) }
    executarReST("ControleEntrega/EnviarMensagemChat", mensagem, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                adicionarMensagemChat(arg.Data, false);
                _mobileChatControleEntrega.Mensagem.val("");
                scroolTextArea();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        }
        else
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
    });

}

function MarcarComoNaoLidoClick(e, sender) {
    var mensagem = { Carga: _mobileChatControleEntrega.Carga.val() }
    executarReST("ControleEntrega/MarcarComoNaoLido", mensagem, function (arg) {
        if (arg.Success) {
            marcarComoNaoLido(_mobileChatControleEntrega.Carga.val());
            limparModalChat();
        }
        else
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
    });
}

function cancelarChatModalClick() {
    limparModalChat();
}

function limparModalChat() {
    $('#' + _mobileChatControleEntrega.Mensagem.id).html("");
    LimparCampos(_mobileChatControleEntrega);
    Global.fecharModal("divModalMobileChatControleEntrega");
}