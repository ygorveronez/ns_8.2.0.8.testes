var EnumSituacaoInfracaoHelper = function () {
    this.Todas = "";
    this.AguardandoProcessamento = 1;
    this.Cancelada = 2;
    this.AguardandoAprovacao = 3;
    this.SemRegraAprovacao = 4;
    this.AprovacaoRejeitada = 5;
    this.Finalizada = 6;
    this.AguardandoConfirmacao = 7;
}

EnumSituacaoInfracaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aguardando Aprovação", value: this.AguardandoAprovacao },
            { text: "Aguardando Processamento", value: this.AguardandoProcessamento },
            { text: "Aprovação Rejeitada", value: this.AprovacaoRejeitada },
            { text: "Cancelada", value: this.Cancelada },
            { text: "Finalizada", value: this.Finalizada },
            { text: "Sem Regra de Aprovação", value: this.SemRegraAprovacao },
            { text: "Integração Aguardando Confirmação", value: this.AguardandoConfirmacao },
            { text: "Todas", value: this.Todas }
        ];
    }
}

var EnumSituacaoInfracao = Object.freeze(new EnumSituacaoInfracaoHelper());