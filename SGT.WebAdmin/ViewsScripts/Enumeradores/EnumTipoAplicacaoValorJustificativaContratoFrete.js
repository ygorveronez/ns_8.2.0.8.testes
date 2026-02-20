var EnumTipoAplicacaoValorJustificativaContratoFreteHelper = function () {
    this.NoAdiantamento = 0;
    this.NoTotal = 1;
    this.NoSaldo = 2;
};

EnumTipoAplicacaoValorJustificativaContratoFreteHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "No Adiantamento do Frete", value: this.NoAdiantamento },
            { text: "No Total do Frete", value: this.NoTotal },
            { text: "No Saldo do Frete", value: this.NoSaldo }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: "" }].concat(this.obterOpcoes());
    }
}

var EnumTipoAplicacaoValorJustificativaContratoFrete = Object.freeze(new EnumTipoAplicacaoValorJustificativaContratoFreteHelper());