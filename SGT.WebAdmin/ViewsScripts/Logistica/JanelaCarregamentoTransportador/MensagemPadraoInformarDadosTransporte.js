/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _mensagemPadraoInformarDadosTransporte;

/*
 * Declaração das Classes
 */

var MensagemPadraoInformarDadosTransporte = function () {
    this.Mensagem = PropertyEntity({ text: "Mensagem:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 1000, enable: false });

    this.Confirmar = PropertyEntity({ eventClick: confirmarLeituraMensagemClick, type: types.event, text: "Entendi" });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadMensagemPadraoInformarDadosTransporte() {
    _mensagemPadraoInformarDadosTransporte = new MensagemPadraoInformarDadosTransporte();
    KoBindings(_mensagemPadraoInformarDadosTransporte, "knockoutMensagemPadraoInformarDadosTransporte");

    carregarMensagemPadraoInformarDadosTransporte();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function confirmarLeituraMensagemClick() {
    fecharModalMensagemPadraoInformarDadosTransporte();
}

/*
 * Declaração das Funções Públicas
 */

function exibirMensagemPadraoInformarDadosTransporte() {
    if (_mensagemPadraoInformarDadosTransporte.Mensagem.val())
        exibirModalMensagemPadraoInformarDadosTransporte();
}

/*
 * Declaração das Funções
 */

function carregarMensagemPadraoInformarDadosTransporte() {
    executarReST("JanelaCarregamentoTransportador/ObterMensagemPadraoInformarDadosTransporte", undefined, function (retorno) {
        if (retorno.Success && retorno.Data) {
            _mensagemPadraoInformarDadosTransporte.Mensagem.val(retorno.Data.MensagemPadraoInformarDadosTransporte);
        }
    });
}

function exibirModalMensagemPadraoInformarDadosTransporte() {
    Global.abrirModal('divModalMensagemPadraoInformarDadosTransporte');
}

function fecharModalMensagemPadraoInformarDadosTransporte() {
    Global.fecharModal('divModalMensagemPadraoInformarDadosTransporte');
}