var EnumTipoBandaRodagemPneuHelper = function () {
    this.Todos = "";
    this.Borrachudo = 1;
    this.Liso = 2;
    this.Macico = 3;
    this.Misto = 4;
    this.Outros = 5;
};

EnumTipoBandaRodagemPneuHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Borrachudo", value: this.Borrachudo },
            { text: "Liso", value: this.Liso },
            { text: "Maciço", value: this.Macico },
            { text: "Misto", value: this.Misto },
            { text: "Outros", value: this.Outros },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoBandaRodagemPneu = Object.freeze(new EnumTipoBandaRodagemPneuHelper());