var EnumStatusBemHelper = function () {
    this.Todos = "";
    this.Aberto = 1;
    this.Finalizado = 2;
    this.Cancelado = 3;
};

EnumStatusBemHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aberto", value: this.Aberto },
            { text: "Finalizado", value: this.Finalizado },
            { text: "Cancelado", value: this.Cancelado },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumStatusBem = Object.freeze(new EnumStatusBemHelper());