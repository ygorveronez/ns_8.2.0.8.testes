/*
 * Declaração de Objetos Globais do Arquivo
 */

var _licitacaoParticipacaoRetornoOferta;

/*
 * Declaração das Classes
 */

var LicitacaoParticipacaoRetornoOferta = function () {
    this.Mensagem = PropertyEntity({ val: ko.observable("Aguardando"), def: "Aguardando" });
    this.ClasseCor = PropertyEntity({ val: ko.observable("alert-warning"), def: "alert-warning" });
    this.Observacao = PropertyEntity({ text: "Observação: " });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadLicitacaoParticipacaoRetornoOferta() {
    _licitacaoParticipacaoRetornoOferta = new LicitacaoParticipacaoRetornoOferta();
    KoBindings(_licitacaoParticipacaoRetornoOferta, "knockoutLicitacaoParticipacaoRetornoOferta");
}

/*
 * Declaração das Funções Públicas
 */

function limparRetornoOferta() {
    LimparCampos(_licitacaoParticipacaoRetornoOferta);
}

function preencherRetornoOferta(dadosRetornoOferta) {
    if (dadosRetornoOferta)
        PreencherObjetoKnout(_licitacaoParticipacaoRetornoOferta, { Data: dadosRetornoOferta });
}