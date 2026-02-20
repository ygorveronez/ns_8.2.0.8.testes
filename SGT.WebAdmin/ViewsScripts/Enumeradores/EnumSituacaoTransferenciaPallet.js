var EnumSituacaoTransferenciaPalletHelper = function () {
    this.Todas = 0;
    this.AguardandoEnvio = 1;
    this.EnvioCancelado = 2;
    this.AguardandoAprovacao = 3;
    this.SemRegraAprovacao = 4;
    this.AprovacaoRejeitada = 5;
    this.AguardandoRecebimento = 6;
    this.Finalizada = 7;
}

EnumSituacaoTransferenciaPalletHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aguardando Aprovação", value: this.AguardandoAprovacao },
            { text: "Aguardando Envio", value: this.AguardandoEnvio },
            { text: "Aguardando Recebimento", value: this.AguardandoRecebimento },
            { text: "Aprovação Rejeitada", value: this.AprovacaoRejeitada },
            { text: "Envio Cancelado", value: this.EnvioCancelado },
            { text: "Finalizada", value: this.Finalizada },
            { text: "Sem Regra de Aprovação", value: this.SemRegraAprovacao },
            { text: "Todas", value: this.Todas }
        ];
    }
}

var EnumSituacaoTransferenciaPallet = Object.freeze(new EnumSituacaoTransferenciaPalletHelper());