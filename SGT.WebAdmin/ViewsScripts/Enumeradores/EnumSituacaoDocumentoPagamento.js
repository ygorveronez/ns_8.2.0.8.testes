var EnumSituacaoDocumentoPagamentoHelper = function () {
    this.Todos = "";
    this.Liberado = 1;
    this.Bloqueado = 2;
};

EnumSituacaoDocumentoPagamentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Liberado", value: this.Liberado },
            { text: "Bloqueado", value: this.Bloqueado },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoDocumentoPagamento = Object.freeze(new EnumSituacaoDocumentoPagamentoHelper());