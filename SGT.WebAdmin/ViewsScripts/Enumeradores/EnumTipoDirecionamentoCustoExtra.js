var EnumTipoDirecionamentoCustoExtraHelper = function () {
    this.Nenhum = 0;
    this.Faturar = 1;
    this.Abonar = 2;
    this.PassThrough = 3;
};

EnumTipoDirecionamentoCustoExtraHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Faturar", value: this.Faturar },
            { text: "Abonar", value: this.Abonar },
            { text: "Pass Through", value: this.PassThrough },
        ];
    },
    obterOpcoesNftp: function () {
        return [
            { text: "Faturar", value: this.Faturar },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Nenhum }].concat(this.obterOpcoes());
    }
};

var EnumTipoDirecionamentoCustoExtra = Object.freeze(new EnumTipoDirecionamentoCustoExtraHelper());
