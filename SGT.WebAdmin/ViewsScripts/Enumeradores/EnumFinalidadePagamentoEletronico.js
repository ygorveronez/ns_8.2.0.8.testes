var EnumFinalidadePagamentoEletronicoHelper = function () {
    this.CreditoContaCorrente = 1;
};

EnumFinalidadePagamentoEletronicoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "01 - Conta corrente individual", value: this.CreditoContaCorrente }
        ];
    }
};

var EnumFinalidadePagamentoEletronico = Object.freeze(new EnumFinalidadePagamentoEletronicoHelper());