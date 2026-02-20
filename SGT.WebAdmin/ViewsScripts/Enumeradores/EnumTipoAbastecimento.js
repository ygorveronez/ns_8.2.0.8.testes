var EnumTipoAbastecimentoHelper = function () {
    this.Todos = 0;
    this.Combustivel = 1;
    this.Arla = 2;
};

EnumTipoAbastecimentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Combustível", value: this.Combustivel },
            { text: "ARLA", value: this.Arla }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoAbastecimento = Object.freeze(new EnumTipoAbastecimentoHelper());