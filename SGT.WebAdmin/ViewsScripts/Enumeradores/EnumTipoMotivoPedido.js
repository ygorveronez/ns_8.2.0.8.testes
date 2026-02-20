var EnumTipoMotivoPedidoHelper = function () {
    this.Todos = "";
    this.LancamentoPedido = 0;
    this.AprovacaoPedido = 1;
    this.RejeicaoPedido = 2;
};

EnumTipoMotivoPedidoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Lançamento Pedido", value: this.LancamentoPedido },
            { text:  "Aprovação Pedido", value: this.AprovacaoPedido },
            { text:   "Rejeição Pedido", value: this.RejeicaoPedido }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumTipoMotivoPedido = Object.freeze(new EnumTipoMotivoPedidoHelper());
