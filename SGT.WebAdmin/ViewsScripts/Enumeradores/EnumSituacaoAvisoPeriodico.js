var EnumSituacaoAvisoPeriodicoHelper = function () {
    this.Todas = "";
    this.AguardandoConfirmacao = 0;
    this.Confirmado = 1;
    this.Rejeitado = 2;
}

EnumSituacaoAvisoPeriodicoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aguardando confirmação", value: this.AguardandoConfirmacao },
            { text: "Confirmado", value: this.Confirmado },
            { text: "Rejeitado", value: this.Rejeitado },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    },
}

var EnumSituacaoAvisoPeriodico = Object.freeze(new EnumSituacaoAvisoPeriodicoHelper());