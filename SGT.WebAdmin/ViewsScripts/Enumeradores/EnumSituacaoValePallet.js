var EnumSituacaoValePalletHelper = function () {
    this.Todas = "";
    this.AgFinalizacao = 1;
    this.AgDevolucao = 4;
    this.Finalizado = 2;
    this.Cancelado = 3;
}

EnumSituacaoValePalletHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Ag. Recolhimento", value: this.AgFinalizacao },
            { text: "Ag. Devolução", value: this.AgDevolucao },
            { text: "Cancelado", value: this.Cancelado },
            { text: "Recolhido", value: this.Finalizado },
            { text: "Todas", value: this.Todas }
        ];
    }
}

var EnumSituacaoValePallet = Object.freeze(new EnumSituacaoValePalletHelper());