let EnumSituacaoPedidoGestaoPedidoHelper = function () {
    this.Todos = 0;
    this.PedidoSemCarga = 1;
    this.PedidoEmSessao = 2;
    this.PedidoComSaldoDisponivel = 3;   
};

EnumSituacaoPedidoGestaoPedidoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Pedido sem carga", value: this.PedidoSemCarga },
            { text: "Pedido em sessão", value: this.PedidoEmSessao },
            { text: "Pedido com saldo disponível", value: this.PedidoComSaldoDisponivel },            
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.SituacaoPedido.Todos, value: this.Todos }].concat(this.obterOpcoes());
    },
};

let EnumSituacaoPedidoGestaoPedido = Object.freeze(new EnumSituacaoPedidoGestaoPedidoHelper());