var EnumSituacaoSinistroHelper = function () {
    this.Todos = 0;
    this.Aberto = 1;
    this.Finalizado = 2
};

EnumSituacaoSinistroHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aberto", value: this.Aberto },
            { text: "Finalizado", value: this.Finalizado },

        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoSinistro = Object.freeze(new EnumSituacaoSinistroHelper());