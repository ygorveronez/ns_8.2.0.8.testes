/*
 * Declaração de Objetos Globais do Arquivo
 */

var _resumoLicitacaoParticipacaoCadastro;

/*
 * Declaração das Classes
 */

var ResumoLicitacaoParticipacaoCadastro = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Numero = PropertyEntity({ text: "Número: " });
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Situacao = PropertyEntity({ text: "Situação: " });
    this.Validade = PropertyEntity({ text: "Validade: " });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadResumoLicitacaoParticipacaoCadastro() {
    _resumoLicitacaoParticipacaoCadastro = new ResumoLicitacaoParticipacaoCadastro();
    KoBindings(_resumoLicitacaoParticipacaoCadastro, "knockoutResumoLicitacaoParticipacao");
}

/*
 * Declaração das Funções Públicas
 */

function limparResumo() {
    LimparCampos(_resumoLicitacaoParticipacaoCadastro);
}

function preencherResumo(dadosResumo) {
    if (dadosResumo)
        PreencherObjetoKnout(_resumoLicitacaoParticipacaoCadastro, { Data: dadosResumo });
}