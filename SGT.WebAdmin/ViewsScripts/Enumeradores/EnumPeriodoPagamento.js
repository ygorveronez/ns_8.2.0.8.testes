var EnumPeriodoPagamentoHelper = function () {
    this.Selecione = "";
    this.Quinzenal = 15;
    this.Mensal = 30;
};

EnumPeriodoPagamentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Quinzenal", value: this.Quinzenal },
            { text: "Mensal", value: this.Mensal }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Selecione", value: this.Selecione }].concat(this.obterOpcoes());
    }
}

var EnumPeriodoPagamento = Object.freeze(new EnumPeriodoPagamentoHelper());