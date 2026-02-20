var EnumTipoLancamentoPneuHelper = function () {
    this.PorQuantidade = 1;
    this.PorFaixa = 2;
};

EnumTipoLancamentoPneuHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Por Quantidade", value: this.PorQuantidade },
            { text: "Por Faixa", value: this.PorFaixa }
        ];
    }
};

var EnumTipoLancamentoPneu = Object.freeze(new EnumTipoLancamentoPneuHelper());