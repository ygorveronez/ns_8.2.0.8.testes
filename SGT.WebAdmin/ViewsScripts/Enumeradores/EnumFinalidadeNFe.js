var EnumFinalidadeNFeHelper = function () {
    this.Todos = "";
    this.Normal = 1;
    this.Complementar = 2;
    this.Ajuste = 3;
    this.Devolucao = 4;
};

EnumFinalidadeNFeHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Normal", value: this.Normal },
            { text: "Complementar", value: this.Complementar },
            { text: "Ajuste", value: this.Ajuste },
            { text: "Devolução", value: this.Devolucao }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumFinalidadeNFe = Object.freeze(new EnumFinalidadeNFeHelper());