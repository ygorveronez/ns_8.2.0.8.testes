var EnumSituacaoOcorrenciaLoteHelper = function () {
    this.Todos = 0;
    this.EmGeracao = 1;
    this.Finalizado = 2;
    this.FalhaNaGeracao = 3;
};

EnumSituacaoOcorrenciaLoteHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Em Geração", value: this.EmGeracao },
            { text: "Finalizado", value: this.Finalizado },
            { text: "Falha na Geração", value: this.FalhaNaGeracao }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoOcorrenciaLote = Object.freeze(new EnumSituacaoOcorrenciaLoteHelper());