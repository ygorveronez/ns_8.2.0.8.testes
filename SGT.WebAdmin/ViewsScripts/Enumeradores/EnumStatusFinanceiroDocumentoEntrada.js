var EnumStatusFinanceiroDocumentoEntradaHelper = function () {
    this.Aberto = 1;
    this.Renegociado = 2;
    this.Pago = 3;
    this.ContratoFinanciamento = 4;
};

EnumStatusFinanceiroDocumentoEntradaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aberto", value: this.Aberto },
            { text: "Renegociado", value: this.Renegociado },
            { text: "Pago", value: this.Pago },
            { text: "Contrato de Financiamento", value: this.ContratoFinanciamento }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: "" }].concat(this.obterOpcoes());
    }
};

var EnumStatusFinanceiroDocumentoEntrada = Object.freeze(new EnumStatusFinanceiroDocumentoEntradaHelper());