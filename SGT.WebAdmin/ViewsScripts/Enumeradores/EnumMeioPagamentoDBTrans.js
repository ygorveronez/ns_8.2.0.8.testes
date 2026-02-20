var EnumMeioPagamentoDBTransHelper = function () {
    this.Todos = "";
    this.Cupom = 1;
    this.CartaoVVP = 3;
    this.ValePedagioAutoExpresso = 4;
    this.ValePedagioTerceiro = 5;
};

EnumMeioPagamentoDBTransHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Cupom", value: this.Cupom },
            { text: "Cartão VVP", value: this.CartaoVVP },
            { text: "Vale Pedágio Auto Expresso", value: this.ValePedagioAutoExpresso },
            { text: "Vale Pedágio Terceiro", value: this.ValePedagioTerceiro }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumMeioPagamentoDBTrans = Object.freeze(new EnumMeioPagamentoDBTransHelper());