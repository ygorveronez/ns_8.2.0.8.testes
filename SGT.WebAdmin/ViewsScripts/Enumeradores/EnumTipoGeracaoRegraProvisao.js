var EnumTipoGeracaoRegraProvisaoHelper = function () {
    this.TermoQuitacao = 1;
    this.PrazoExcedido = 2;
}

EnumTipoGeracaoRegraProvisaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Termo Quitação", value: this.TermoQuitacao },
            { text: "Prazo Excedido", value: this.PrazoExcedido }
        ];
    }
}

var EnumTipoGeracaoRegraProvisao = Object.freeze(new EnumTipoGeracaoRegraProvisaoHelper());