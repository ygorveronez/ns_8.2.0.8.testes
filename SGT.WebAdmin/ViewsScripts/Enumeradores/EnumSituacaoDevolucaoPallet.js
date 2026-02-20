var EnumSituacaoDevolucaoPalletHelper = function () {
    this.Todas = "";
    this.AgEntrega = 0;
    this.Entregue = 1;
    this.Cancelado = 2;
};

EnumSituacaoDevolucaoPalletHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Ag. Entrega", value: this.AgEntrega },
            { text: "Entregue", value: this.Entregue }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoDevolucaoPallet = Object.freeze(new EnumSituacaoDevolucaoPalletHelper());