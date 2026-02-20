var EnumSituacaoValeAvulsoHelper = function () {
    this.Todos = 0;
    this.Aberto = 1;
    this.Cancelado = 2;
    this.Finalizado = 3;
};

EnumSituacaoValeAvulsoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aberto", value: this.Aberto },
            { text: "Cancelado", value: this.Cancelado },
            { text: "Finalizado", value: this.Finalizado },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoValeAvulso = Object.freeze(new EnumSituacaoValeAvulsoHelper());