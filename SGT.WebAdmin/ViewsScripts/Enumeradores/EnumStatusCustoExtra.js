var EnumStatusCustoExtraHelper = function () {
    this.Todos = "";
    this.Abonado = 0;
    this.Faturado = 1;
    this.PassThrough = 2;
    this.EmAberto = 3;
};

EnumStatusCustoExtraHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Abonado", value: this.Abonado },
            { text: "Faturado", value: this.Faturado },
            { text: "Pass Through", value: this.PassThrough },
            { text: "Em Aberto", value: this.EmAberto },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [
            { text: "Todos", value: this.Todos },
            { text: "Faturado", value: this.Faturado },
            { text: "Em Aberto", value: this.EmAberto },
        ];
    }
}

var EnumStatusCustoExtra = Object.freeze(new EnumStatusCustoExtraHelper());