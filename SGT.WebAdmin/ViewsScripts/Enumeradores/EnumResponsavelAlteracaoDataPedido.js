var EnumResponsavelAlteracaoDataPedidoHelper = function () {
    this.Embarcador = 1;
    this.Transportador = 2;
};

EnumResponsavelAlteracaoDataPedidoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Embarcador", value: this.Embarcador },
            { text: "Transportador", value: this.Transportador }
        ];
    }
}

var EnumResponsavelAlteracaoDataPedido = Object.freeze(new EnumResponsavelAlteracaoDataPedidoHelper());