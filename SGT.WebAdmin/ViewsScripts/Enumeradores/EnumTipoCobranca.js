var EnumTipoCobrancaHelper = function () {
    this.Banco = 1;
    this.Carteira = 2;
    this.Cartao = 3;
    this.Cheque = 4;
};

EnumTipoCobrancaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Banco", value: this.Banco },
            { text: "Carteira", value: this.Carteira }
        ];
    }
};

var EnumTipoCobranca = Object.freeze(new EnumTipoCobrancaHelper());