/// <reference path="Transferencia.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _cruzamentoInformacoesTransferenciaPallet;

/*
 * Declaração das Classes
 */

var CruzamentoInformacoesTransferenciaPallet = function () {
    this.QuantidadeEnviada = PropertyEntity({ val: ko.observable(""), def: "", text: "Quantidade Enviada: ", getType: typesKnockout.int });
    this.QuantidadeRecebida = PropertyEntity({ val: ko.observable(""), def: "", text: "Quantidade Recebida: ", getType: typesKnockout.int });
    this.QuantidadeSolicitada = PropertyEntity({ val: ko.observable(""), def: "", text: "Quantidade Solicitada: ", getType: typesKnockout.int });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadCruzamentoInformacoesTransferenciaPallet() {
    _cruzamentoInformacoesTransferenciaPallet = new CruzamentoInformacoesTransferenciaPallet();
    KoBindings(_cruzamentoInformacoesTransferenciaPallet, "knockoutCruzamentoInformacoesTransferenciaPallet");
}

/*
 * Declaração das Funções
 */

function limparCruzamentoInformacoes() {
    LimparCampos(_cruzamentoInformacoesTransferenciaPallet);
}

function preencherQuantidadeEnviada(quantidadeEnviada) {
    _cruzamentoInformacoesTransferenciaPallet.QuantidadeEnviada.val(quantidadeEnviada);
}

function preencherQuantidadeRecebida(quantidadeRecebimento) {
    _cruzamentoInformacoesTransferenciaPallet.QuantidadeRecebida.val(quantidadeRecebimento);
}

function preencherQuantidadeSolicitada(quantidadeSolicitada) {
    _cruzamentoInformacoesTransferenciaPallet.QuantidadeSolicitada.val(quantidadeSolicitada);
}