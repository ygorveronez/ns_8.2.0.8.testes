var EnumFormaAutorizacaoPagamentoChamadoHelper = function () {
    this.Todos = 0;
    this.AutorizarValorRecibo = 1;
    this.AutorizarValorChamado = 2;
    this.PagamentoNaoAutorizado = 3;
    this.AutorizarValorDiferente = 4;
};

EnumFormaAutorizacaoPagamentoChamadoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Autorizar Valor do Recibo", value: this.AutorizarValorRecibo },
            { text: "Autorizar Valor do Chamado", value: this.AutorizarValorChamado },
            { text: "Pagamento Não Autorizado", value: this.PagamentoNaoAutorizado },
            { text: "Autorizar Valor Diferente", value: this.AutorizarValorDiferente }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumFormaAutorizacaoPagamentoChamado = Object.freeze(new EnumFormaAutorizacaoPagamentoChamadoHelper());