var EnumTipoMotivoRejeicaoAlteracaoPedidoHelper = function () {
    this.Todos = 0;
    this.Embarcador = 1;
    this.Transportador = 2;
};

EnumTipoMotivoRejeicaoAlteracaoPedidoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Todos", value: this.Todos },
            { text: "Embarcador", value: this.Embarcador },
            { text: "Transportador", value: this.Transportador }
        ];
    }
}

var EnumTipoMotivoRejeicaoAlteracaoPedido = Object.freeze(new EnumTipoMotivoRejeicaoAlteracaoPedidoHelper());