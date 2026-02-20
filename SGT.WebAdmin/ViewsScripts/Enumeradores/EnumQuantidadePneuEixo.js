var EnumQuantidadePneuEixoHelper = function () {
    this.Todos = "";
    this.Duplo = 1;
    this.Simples = 2;
};

EnumQuantidadePneuEixoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Duplo", value: this.Duplo },
            { text: "Simples", value: this.Simples }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumQuantidadePneuEixo = Object.freeze(new EnumQuantidadePneuEixoHelper());