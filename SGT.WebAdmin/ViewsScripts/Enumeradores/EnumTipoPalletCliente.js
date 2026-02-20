var EnumTipoPalletClienteHelper = function () {
    this.NaoDefinido = 0,
    this.Chep = 1,
    this.Batido = 2,
    this.PaleteRetorno = 3
}

EnumTipoPalletClienteHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "", value: this.NaoDefinido },
            { text: "Chep", value: this.Chep },
            { text: "Batido", value: this.Batido },
            { text: "Palete Retorno", value: this.PaleteRetorno },
        ];
    },
    obterDescricao: function (valor) {
        var opcoes = this.obterOpcoes();
        var opcao = opcoes.find(function (op) {
            return op.value === valor;
        });
        return opcao ? opcao.text : "";
    }
}

var EnumTipoPalletCliente = Object.freeze(new EnumTipoPalletClienteHelper());