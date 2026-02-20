var EnumFormaPagamentoCIOTHelper = function () {
    this.NaoSelecionado = "";
    this.AVista = 0;
    this.APrazo = 1;
};

EnumFormaPagamentoCIOTHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "À vista", value: this.AVista },
            { text: "À prazo", value: this.APrazo },
            { text: "Não selecionado", value: this.NaoSelecionado },
        ];
    }
};

var EnumFormaPagamentoCIOT = Object.freeze(new EnumFormaPagamentoCIOTHelper());