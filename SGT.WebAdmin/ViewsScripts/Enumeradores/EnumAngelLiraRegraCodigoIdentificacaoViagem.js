var EnumAngelLiraRegraCodigoIdentificacaoViagemHelper = function () {
    this.NumeroPedidoEmbarcadorMaisPlaca = 1;
    this.IDCargaMaisNumeroPedidoEmbarcador = 2;
};

EnumAngelLiraRegraCodigoIdentificacaoViagemHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Número pedido do embarcador + placa", value: this.NumeroPedidoEmbarcadorMaisPlaca },
            { text: "ID da carga + número pedido embarcador", value: this.IDCargaMaisNumeroPedidoEmbarcador },
        ];
    }
};

var EnumAngelLiraRegraCodigoIdentificacaoViagem = Object.freeze(new EnumAngelLiraRegraCodigoIdentificacaoViagemHelper());