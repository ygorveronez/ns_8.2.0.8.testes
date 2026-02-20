var EnumSituacaoOcorrenciaPatioHelper = function () {
    this.Todas = "";
    this.Pendente = 1;
    this.Aprovada = 2;
    this.Reprovada = 3;
}

EnumSituacaoOcorrenciaPatioHelper.prototype = {
    obterListaOpcaoPendente: function () {
        return [this.Pendente];
    },
    obterOpcoes: function () {
        return [
            { text: "Aprovada", value: this.Aprovada },
            { text: "Pendente", value: this.Pendente },
            { text: "Reprovada", value: this.Reprovada }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoOcorrenciaPatio = Object.freeze(new EnumSituacaoOcorrenciaPatioHelper());