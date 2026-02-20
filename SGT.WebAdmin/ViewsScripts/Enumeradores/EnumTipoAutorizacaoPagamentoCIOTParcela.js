var EnumTipoAutorizacaoPagamentoCIOTParcelaHelper = function () {
    this.Adiantamento = 1;
    this.Abastecimento = 2;
    this.Saldo = 3;
};

EnumTipoAutorizacaoPagamentoCIOTParcelaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Adiantamento", value: this.Adiantamento },
            { text: "Abastecimento", value: this.Abastecimento },
            { text: "Saldo", value: this.Saldo }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoAutorizacaoPagamentoCIOTParcela = Object.freeze(new EnumTipoAutorizacaoPagamentoCIOTParcelaHelper());