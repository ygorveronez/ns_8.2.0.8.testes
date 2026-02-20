/// <reference path="Transferencia.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _resumoTransferenciaPallet;

/*
 * Declaração das Classes
 */

var ResumoTransferenciaPallet = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Data = PropertyEntity({ text: "Data: " });
    this.Filial = PropertyEntity({ text: "Filial: " });
    this.Numero = PropertyEntity({ text: "Número da Solicitação: ", visible: ko.observable(false) });
    this.Quantidade = PropertyEntity({ text: "Quantidade: " });
    this.Setor = PropertyEntity({ text: "Setor: " });
    this.Situacao = PropertyEntity({ text: "Situação: " });
    this.Solicitante = PropertyEntity({ text: "Solicitante: " });
    this.Turno = PropertyEntity({ text: "Turno: " });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadResumoTransferenciaPallet() {
    _resumoTransferenciaPallet = new ResumoTransferenciaPallet();
    KoBindings(_resumoTransferenciaPallet, "knockoutResumoTransferenciaPallet");
}

/*
 * Declaração das Funções Públicas
 */

function limparResumo() {
    _resumoTransferenciaPallet.Numero.visible(false);

    LimparCampos(_resumoTransferenciaPallet);
}

function preencherResumo(dadosResumo) {
    PreencherObjetoKnout(_resumoTransferenciaPallet, { Data: dadosResumo });

    _resumoTransferenciaPallet.Numero.visible(true);
}