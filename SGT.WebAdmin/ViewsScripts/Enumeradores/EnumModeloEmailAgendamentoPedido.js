var EnumModeloEmailAgendamentoPedidoHelper = function () {
    this.Modelo1 = 1;
    this.Modelo2 = 2;
};

EnumModeloEmailAgendamentoPedidoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Modelo 1", value: this.Modelo1 },
            { text: "Modelo 2", value: this.Modelo2 }
        ];
    }
};

var EnumModeloEmailAgendamentoPedido = Object.freeze(new EnumModeloEmailAgendamentoPedidoHelper());