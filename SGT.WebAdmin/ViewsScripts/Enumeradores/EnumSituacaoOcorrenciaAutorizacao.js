var EnumSituacaoOcorrenciaAutorizacaoHelper = function () {
    this.Todos = "";
    this.Pendente = 0;
    this.Aprovada = 1;
    this.Rejeitada = 9;
};

EnumSituacaoOcorrenciaAutorizacaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Pendente", value: this.Pendente },
            { text: "Aprovada", value: this.Aprovada },
            { text: "Rejeitada", value: this.Rejeitada }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoOcorrenciaAutorizacao = Object.freeze(new EnumSituacaoOcorrenciaAutorizacaoHelper());