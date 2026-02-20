var EnumEntradaSaidaHelper = function () {
    this.Todos = 0;
    this.Entrada = 1;
    this.Saida = 2;
};

EnumEntradaSaidaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Entrada", value: this.Entrada },
            { text: "Saída", value: this.Saida }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumEntradaSaida = Object.freeze(new EnumEntradaSaidaHelper());