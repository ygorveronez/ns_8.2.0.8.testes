var EnumTipoContatoAgendamentoEntregaPedidoHelper = function () {
    this.Transportador = 1;
    this.Cliente = 2;
};

EnumTipoContatoAgendamentoEntregaPedidoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Transportador", value: this.Transportador },
            { text: "Cliente", value: this.Cliente }
        ];
    }
}

var EnumTipoContatoAgendamentoEntregaPedido = Object.freeze(new EnumTipoContatoAgendamentoEntregaPedidoHelper());
