var EnumTipoContaPagamentoEletronicoHelper = function () {
    this.ContaCorrenteIndividual = 1;
    this.TDSTransferencia = 3;
};

EnumTipoContaPagamentoEletronicoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "01 - Crédito em conta corrente", value: this.ContaCorrenteIndividual },
            { text: "03 - TDS Transferência", value: this.TDSTransferencia }
        ];
    }
};

var EnumTipoContaPagamentoEletronico = Object.freeze(new EnumTipoContaPagamentoEletronicoHelper());