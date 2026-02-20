/// <reference path="Reforma.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _nfeRetornoResumoReformaPallet;

/*
 * Declaração das Classes
 */

var NfeRetornoResumoReformaPallet = function () {
    this.QuantidadeEnviada = PropertyEntity({ val: ko.observable(""), def: "", text: "Quantidade Enviada: ", getType: typesKnockout.int });
    this.QuantidadeRecebida = PropertyEntity({ val: ko.observable(""), def: "", text: "Quantidade Recebida: ", getType: typesKnockout.int });
    this.Valor = PropertyEntity({ val: ko.observable(""), def: "", text: "Valor NF-e: ", getType: typesKnockout.decimal });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadNfeRetornoResumoReformaPallet() {
    _nfeRetornoResumoReformaPallet = new NfeRetornoResumoReformaPallet();
    KoBindings(_nfeRetornoResumoReformaPallet, "knockoutNfeRetornoResumoReformaPallet");
}

/*
 * Declaração das Funções
 */

function limparNfeRetornoResumo() {
    LimparCampos(_nfeRetornoResumoReformaPallet);
}

function preencherNfeRetornoResumoDadosNfe(listaNfeRetorno) {
    var quantidade = 0;
    var valor = 0;

    for (var i = 0; i < listaNfeRetorno.length; i++) {
        var nfeRetorno = listaNfeRetorno[i];

        quantidade += nfeRetorno.Quantidade;
        valor += nfeRetorno.Valor;
    }

    _nfeRetornoResumoReformaPallet.QuantidadeRecebida.val(quantidade);
    _nfeRetornoResumoReformaPallet.Valor.val(Globalize.format(valor, "n2"));
}

function preencherNfeRetornoResumoQuantidadeEnvio(quantidadeEnvio) {
    _nfeRetornoResumoReformaPallet.QuantidadeEnviada.val(quantidadeEnvio);
}