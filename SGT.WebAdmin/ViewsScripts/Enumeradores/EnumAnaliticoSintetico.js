var EnumAnaliticoSinteticoHelper = function () {
    this.Todos = "";
    this.Analitico = 1;
    this.Sintetico = 2;
};

EnumAnaliticoSinteticoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Analítico", value: this.Analitico },
            { text: "Sintético", value: this.Sintetico }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumAnaliticoSintetico = Object.freeze(new EnumAnaliticoSinteticoHelper());