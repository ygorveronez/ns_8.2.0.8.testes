let EnumTipoPagamentoAutorizacaoPagamentoHelper = function () {
    this.Todas = 0;
    this.PagamentoAdiantamento = 1;
    this.PagamentoSaldo = 2;
};

EnumTipoPagamentoAutorizacaoPagamentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Pagamento Adiantamento", value: this.PagamentoAdiantamento },
            { text: "Pagamento Saldo", value: this.PagamentoSaldo }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
};

let EnumTipoPagamentoAutorizacaoPagamento = Object.freeze(new EnumTipoPagamentoAutorizacaoPagamentoHelper());