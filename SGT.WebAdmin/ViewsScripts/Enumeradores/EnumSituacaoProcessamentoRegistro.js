const EnumSituacaoProcessamentoRegistroHelper = function () {
    this.Todas = null;
    this.Pendente = 0;
    this.Sucesso = 1;
    this.Falha = 2;
    this.Liberado = 3;
    this.FalhaLiberacao = 4;
};

EnumSituacaoProcessamentoRegistroHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Pendente", value: this.Pendente },
            { text: "Sucesso", value: this.Sucesso },
            { text: "Falha", value: this.Falha },
            { text: "Liberado", value: this.Liberado },
            { text: "Falha Liberação", value: this.FalhaLiberacao },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todas }].concat(this.obterOpcoes());
    }
}

const EnumSituacaoProcessamentoRegistro = Object.freeze(new EnumSituacaoProcessamentoRegistroHelper());
