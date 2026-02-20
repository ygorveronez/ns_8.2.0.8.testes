var EnumTipoFechamentoFreteHelper = function () {
    this.Todos = "";
    this.FechamentoPorKm = 1;
    this.FechamentoPorFaixaKm = 2;
};

EnumTipoFechamentoFreteHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Fechamento por faixa de km", value: this.FechamentoPorFaixaKm },
            { text: "Fechamento por km", value: this.FechamentoPorKm }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumTipoFechamentoFrete = Object.freeze(new EnumTipoFechamentoFreteHelper());