var EnumSituacaoLiberacaoPagamentoProvedorHelper = function () {
    this.Todos = 0;
    this.Aberto = 1;
    this.Rejeitada = 2;
    this.Finalizada = 3;
    this.Cancelada = 4;
};

EnumSituacaoLiberacaoPagamentoProvedorHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Todos", value: this.Todos },
            { text: "Aberto", value: this.Aberto },
            { text: "Rejeitada", value: this.Rejeitada },
            { text: "Finalizada", value: this.Finalizada },
            { text: "Cancelada", value: this.Cancelada },
        ];
    }
};

var EnumSituacaoLiberacaoPagamentoProvedor = Object.freeze(new EnumSituacaoLiberacaoPagamentoProvedorHelper());