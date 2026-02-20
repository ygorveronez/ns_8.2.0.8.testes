/// <reference path="reforma.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _resumoReformaPallet;

/*
 * Declaração das Classes
 */

var ResumoReformaPallet = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Data = PropertyEntity({ text: "Data: " });
    this.Filial = PropertyEntity({ text: "Filial: ", visible: !_isTMS });
    this.Fornecedor = PropertyEntity({ text: "Fornecedor: " });
    this.Numero = PropertyEntity({ text: "Número da Reforma: ", visible: ko.observable(false) });
    this.Situacao = PropertyEntity({ text: "Situação: " });
    this.Transportador = PropertyEntity({ text: "Empresa/Filial: ", visible: _isTMS });
    this.QuantidadesEnviadas = ko.observableArray();
}

var ResumoReformaPalletQuantidade = function (situacao) {
    this.Quantidade = PropertyEntity({ val: situacao.Quantidade, getType: typesKnockout.int, def: "", text: situacao.Descricao + ":" });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadResumoTransferenciaPallet() {
    _resumoReformaPallet = new ResumoReformaPallet();
    KoBindings(_resumoReformaPallet, "knockoutResumoReformaPallet");
}

/*
 * Declaração das Funções Públicas
 */

function carregarQuantidadesEnviadas(quantidadesEnviadas) {
    for (var i = 0; i < quantidadesEnviadas.length; i++) {
        var quantidadeEnviada = new ResumoReformaPalletQuantidade(quantidadesEnviadas[i]);

        _resumoReformaPallet.QuantidadesEnviadas.push(quantidadeEnviada);
    }
}

function limparResumo() {
    _resumoReformaPallet.Numero.visible(false);

    LimparCampos(_resumoReformaPallet);

    _resumoReformaPallet.QuantidadesEnviadas.removeAll();
}

function preencherResumo(dadosResumo) {
    PreencherObjetoKnout(_resumoReformaPallet, { Data: dadosResumo });

    carregarQuantidadesEnviadas(dadosResumo.QuantidadesEnviadas);

    _resumoReformaPallet.Numero.visible(true);
}