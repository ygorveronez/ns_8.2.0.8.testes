var EnumModoAbastecimentoHelper = function () {
    this.Todos = 0;
    this.Interno = 1;
};

EnumModoAbastecimentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Interno", value: this.Interno }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumModoAbastecimento = Object.freeze(new EnumModoAbastecimentoHelper());