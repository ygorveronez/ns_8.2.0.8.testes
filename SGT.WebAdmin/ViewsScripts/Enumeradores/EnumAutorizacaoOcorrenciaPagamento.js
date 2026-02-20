var EnumAutorizacaoOcorrenciaPagamentoHelper = function () {
    this.Todos = "";
    this.Remetente = 1;
    this.Destinatario = 2;
    this.ConfirmarPagador = 3;
};

EnumAutorizacaoOcorrenciaPagamentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Remetente", value: this.Remetente },
            { text: "Destinatário", value: this.Destinatario },
            { text: "Confirmar Pagador", value: this.ConfirmarPagador }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumAutorizacaoOcorrenciaPagamento = Object.freeze(new EnumAutorizacaoOcorrenciaPagamentoHelper());