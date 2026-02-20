var EnumIndicadorPagamentoDocumentoEntradaHelper = function () {
    this.AVista = 0;
    this.APrazo = 1;
    this.Outros = 9;
};

EnumIndicadorPagamentoDocumentoEntradaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "À vista", value: this.AVista },
            { text: "À prazo", value: this.APrazo },
            { text: "Outros", value: this.Outros }
        ];
    }
};

var EnumIndicadorPagamentoDocumentoEntrada = Object.freeze(new EnumIndicadorPagamentoDocumentoEntradaHelper());