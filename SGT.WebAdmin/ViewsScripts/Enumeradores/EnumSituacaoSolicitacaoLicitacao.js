var EnumSituacaoSolicitacaoLicitacaoHelper = function () {
    this.Todos = 0;
    this.AgCotacao = 1;
    this.Finalizada = 2;
    this.Rejeitada = 3;
    this.Cancelada = 4;
};

EnumSituacaoSolicitacaoLicitacaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Ag. Cotação", value: this.AgCotacao },
            { text: "Finalizada", value: this.Finalizada },
            { text: "Rejeitada", value: this.Rejeitada },
            { text: "Cancelada", value: this.Cancelada }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumSituacaoSolicitacaoLicitacao = Object.freeze(new EnumSituacaoSolicitacaoLicitacaoHelper());