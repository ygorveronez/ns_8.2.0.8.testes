var EnumSituacaoOnTimeHelper = function () {
    this.NaoAjustado = 0;
    this.ForaDoPrazo = 1;
    this.NoPrazo = 2;
};

EnumSituacaoOnTimeHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Não Ajustado", value: this.NaoAjustado },
            { text: "Not On Time (Fora do Prazo)", value: this.ForaDoPrazo },
            { text: "On Time (No Prazo)", value: this.NoPrazo }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoOnTime = Object.freeze(new EnumSituacaoOnTimeHelper());