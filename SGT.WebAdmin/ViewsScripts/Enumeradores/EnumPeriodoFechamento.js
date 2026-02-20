var EnumPeriodoFechamentoHelper = function () {
    this.Todos = "";
    this.Decendial = 10;
    this.Quinzenal = 15;
    this.Mensal = 30;
};

EnumPeriodoFechamentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Decendial", value: this.Decendial },
            { text: "Quinzenal", value: this.Quinzenal },
            { text: "Mensal", value: this.Mensal }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumPeriodoFechamento = Object.freeze(new EnumPeriodoFechamentoHelper());