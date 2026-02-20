var EnumSituacaoLancamentoNFSManualHelper = function () {
    this.Todas = 0;
    this.DadosNota = 1;
    this.AgAprovacao = 2;
    this.Reprovada = 3;
    this.AgIntegracao = 4;
    this.SemRegra = 5;
    this.AgEmissao = 6;
    this.EmEmissao = 7;
    this.FalhaEmissao = 8;
    this.FalhaIntegracao = 9;
    this.Finalizada = 10;
    this.Cancelada = 11;
    this.Anulada = 12;
};

EnumSituacaoLancamentoNFSManualHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Ag. Aprovação", value: this.AgAprovacao },
            { text: "Ag Emissão", value: this.AgEmissao },
            { text: "Ag. Integração", value: this.AgIntegracao },
            { text: "Anulada", value: this.Anulada },
            { text: "Cancelada", value: this.Cancelada },
            { text: "Dados da Nota", value: this.DadosNota },
            { text: "Em Emissão", value: this.EmEmissao },
            { text: "Falha na Emissão", value: this.FalhaEmissao },
            { text: "Falha na Integração", value: this.FalhaIntegracao },
            { text: "Finalizada", value: this.Finalizada },
            { text: "Reprovada", value: this.Reprovada },
            { text: "Sem Regra de Aprovação", value: this.SemRegra }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
}

var EnumSituacaoLancamentoNFSManual = Object.freeze(new EnumSituacaoLancamentoNFSManualHelper());