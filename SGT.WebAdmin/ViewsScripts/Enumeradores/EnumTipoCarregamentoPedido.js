var EnumTipoCarregamentoPedidoHelper = function () {
    this.NaoDefinido = 0;
    this.Normal = 1;
    this.TrocaNota = 2;
};

EnumTipoCarregamentoPedidoHelper.prototype = {
    obterDescricao: function (tipo) {
        switch (tipo) {
            case this.Normal: return "Normal";
            case this.TrocaNota: return "Troca de Nota";
            default: return "Não Definido";
        }
    },
    obterOpcoes: function () {
        return [
            { text: "Não Definido", value: this.NaoDefinido },
            { text: "Normal", value: this.Normal },
            { text: "Troca de Nota", value: this.TrocaNota }
        ];
    }
}

var EnumTipoCarregamentoPedido = Object.freeze(new EnumTipoCarregamentoPedidoHelper());
