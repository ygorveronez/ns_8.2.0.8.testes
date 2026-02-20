var EnumPedidoSessaoHelper = function () {
    this.Todos = 1;
    this.Regular = 2;
    this.Reentrega = 3
};

EnumPedidoSessaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.PedidosSessao.Todos, value: this.Todos },
            { text: Localization.Resources.Enumeradores.PedidosSessao.Regular, value: this.Regular },
            { text: Localization.Resources.Enumeradores.PedidosSessao.Reentrega, value: this.Reentrega },
        ];
    }
}

var EnumPedidoSessao = Object.freeze(new EnumPedidoSessaoHelper());