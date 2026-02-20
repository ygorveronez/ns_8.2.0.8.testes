var EnumSituacaoAvariaPalletHelper = function () {
    this.Todas = 0;
    this.AguardandoAprovacao = 1;
    this.SemRegraAprovacao = 2;
    this.AprovacaoRejeitada = 3;
    this.Finalizada = 4;
}

EnumSituacaoAvariaPalletHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aguardando Aprovação", value: this.AguardandoAprovacao },
            { text: "Aprovação Rejeitada", value: this.AprovacaoRejeitada },
            { text: "Finalizada", value: this.Finalizada },
            { text: "Sem Regra de Aprovação", value: this.SemRegraAprovacao },
            { text: "Todas", value: this.Todas }
        ];
    }
}

var EnumSituacaoAvariaPallet = Object.freeze(new EnumSituacaoAvariaPalletHelper());