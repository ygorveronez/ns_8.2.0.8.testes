var EnumSituacaoConferenciaContainerHelper = function () {
    this.Todas = "";
    this.AguardandoContainer = 1;
    this.AguardandoConferencia = 2;
    this.ConferenciaAprovada = 3;
};

EnumSituacaoConferenciaContainerHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aguardando Conferência", value: this.AguardandoConferencia },
            { text: "Aguardando Container", value: this.AguardandoContainer },
            { text: "Conferência Aprovada", value: this.ConferenciaAprovada }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
}

var EnumSituacaoConferenciaContainer = Object.freeze(new EnumSituacaoConferenciaContainerHelper());
