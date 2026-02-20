var EnumTipoEmissaoNFeHelper = function () {
    this.Todos = -1;
    this.Entrada = 0;
    this.Saida = 1;
};

EnumTipoEmissaoNFeHelper.prototype = {
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

var EnumTipoEmissaoNFe = Object.freeze(new EnumTipoEmissaoNFeHelper());