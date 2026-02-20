/// <reference path="Reforma.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _nfeSaidaResumoReformaPallet;

/*
 * Declaração das Classes
 */

var NfeSaidaResumoReformaPallet = function () {
    this.QuantidadeEnvio = PropertyEntity({ val: ko.observable(""), def: "", text: "Quantidade Envio: ", getType: typesKnockout.int });
    this.QuantidadeNfe = PropertyEntity({ val: ko.observable(""), def: "", text: "Quantidade NF-e: ", getType: typesKnockout.int });
    this.ValorNfe = PropertyEntity({ val: ko.observable(""), def: "", text: "Valor NF-e: ", getType: typesKnockout.decimal });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadNfeSaidaResumoReformaPallet() {
    _nfeSaidaResumoReformaPallet = new NfeSaidaResumoReformaPallet();
    KoBindings(_nfeSaidaResumoReformaPallet, "knockoutNfeSaidaResumoReformaPallet");
}

/*
 * Declaração das Funções
 */

function limparNfeSaidaResumo() {
    LimparCampos(_nfeSaidaResumoReformaPallet);
}

function preencherNfeSaidaResumoDadosNfe(listaNfeSaida) {
    var quantidade = 0;
    var valor = 0;

    for (var i = 0; i < listaNfeSaida.length; i++) {
        var nfeSaida = listaNfeSaida[i];

        quantidade += nfeSaida.Quantidade;
        valor += nfeSaida.Valor;
    }

    _nfeSaidaResumoReformaPallet.QuantidadeNfe.val(quantidade);
    _nfeSaidaResumoReformaPallet.ValorNfe.val(Globalize.format(valor, "n2"));
}

function preencherNfeSaidaResumoQuantidadeEnvio(quantidadeEnvio) {
    _nfeSaidaResumoReformaPallet.QuantidadeEnvio.val(quantidadeEnvio);
}