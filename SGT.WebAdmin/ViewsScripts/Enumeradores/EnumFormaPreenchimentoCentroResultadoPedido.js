var EnumFormaPreenchimentoCentroResultadoPedidoHelper = function () {
    this.TipoOperacao = 0;
    this.Veiculo = 1;
};

EnumFormaPreenchimentoCentroResultadoPedidoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Por Tipo de Operação", value: this.TipoOperacao },
            { text: "Por Veículo", value: this.Veiculo }
        ];
    }
};

var EnumFormaPreenchimentoCentroResultadoPedido = Object.freeze(new EnumFormaPreenchimentoCentroResultadoPedidoHelper());