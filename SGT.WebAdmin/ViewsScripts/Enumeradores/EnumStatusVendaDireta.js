var EnumStatusVendaDiretaHelper = function () {
    this.Todos = "";
    this.Pendente = 1;
    this.Finalizado = 2;
    this.Cancelado = 3;
};

EnumStatusVendaDiretaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Pendente", value: this.Pendente },
            { text: "Finalizado", value: this.Finalizado },
            { text: "Cancelado", value: this.Cancelado },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumStatusVendaDireta = Object.freeze(new EnumStatusVendaDiretaHelper());