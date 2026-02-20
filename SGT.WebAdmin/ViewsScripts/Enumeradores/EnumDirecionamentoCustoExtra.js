var EnumDirecionamentoCustoExtraHelper = function () {
    this.Todos = "";
    this.Abonar = 0;
    this.Faturar = 1;
    this.PassThrough = 2;
};

EnumDirecionamentoCustoExtraHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Abonar", value: this.Abonar },
            { text: "Faturar", value: this.Faturar },
            { text: "Pass Through", value: this.PassThrough },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [
            { text: "Todos", value: this.Todos },
            { text: "Faturar", value: this.Faturar },
        ];
    }
}

var EnumDirecionamentoCustoExtra = Object.freeze(new EnumDirecionamentoCustoExtraHelper());