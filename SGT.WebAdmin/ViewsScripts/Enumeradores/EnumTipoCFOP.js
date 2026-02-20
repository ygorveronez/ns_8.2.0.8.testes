
var EnumTipoCFOP = { Entrada: 0, Saida: 1 };

var EnumTipoCFOPHelper = function () {
    this.Entrada = 0;
    this.Saida = 1;
};

EnumTipoCFOPHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Entrada", value: this.Entrada },
            { text: "Saída", value: this.Saida }
        ];
    },
    obterOpcoesPesquisa: function () {
        return this.obterOpcoes();
    },

};

var EnumTipoCFOPPesquisa = Object.freeze(new EnumTipoCFOPHelper());