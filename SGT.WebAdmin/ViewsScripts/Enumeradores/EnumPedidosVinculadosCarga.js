var EnumPedidosVinculadosCargaHelper = function () {
    this.PedidoComCarga = 0;
    this.PedidoSemCarga = 1;
    this.PedidoCargaComMotoristaVinculado = 2;
    this.PedidoCargaSemMotoristaVinculado = 3;
    this.PedidoCargaComVeiculoVinculado = 4;
    this.PedidoCargaSemVeiculoVinculado = 5;
};

EnumPedidosVinculadosCargaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Pedido com carga", value: this.PedidoComCarga },
            { text: "Pedido sem carga", value: this.PedidoSemCarga },
            { text: "Pedido/Carga com motorista vinculado", value: this.PedidoCargaComMotoristaVinculado },
            { text: "Pedido/Carga sem motorista vinculado", value: this.PedidoCargaSemMotoristaVinculado },
            { text: "Pedido/Carga com veículo vinculado", value: this.PedidoCargaComVeiculoVinculado },
            { text: "Pedido/Carga sem veículo vinculado", value: this.PedidoCargaSemVeiculoVinculado },
        ];
    },
}

var EnumPedidosVinculadosCarga = Object.freeze(new EnumPedidosVinculadosCargaHelper());