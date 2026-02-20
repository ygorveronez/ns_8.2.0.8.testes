/*
 * Declaração de Objetos Globais do Arquivo
 */

var _resumoLicitacaoParticipacao;

/*
 * Declaração das Classes
 */

var ResumoLicitacaoParticipacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Numero = PropertyEntity({ text: "Número: " });
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Situacao = PropertyEntity({ text: "Situação: " });
    this.Transportador = PropertyEntity({ text: "Transportador: " });
    this.Validade = PropertyEntity({ text: "Validade: " });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadResumoLicitacaoParticipacao() {
    _resumoLicitacaoParticipacao = new ResumoLicitacaoParticipacao();
    KoBindings(_resumoLicitacaoParticipacao, "knockoutResumoLicitacaoParticipacaoAvaliacao");
}

/*
 * Declaração das Funções Públicas
 */

function limparResumo() {
    LimparCampos(_resumoLicitacaoParticipacao);
}

function preencherResumo(dadosResumo) {
    PreencherObjetoKnout(_resumoLicitacaoParticipacao, { Data: dadosResumo });
}