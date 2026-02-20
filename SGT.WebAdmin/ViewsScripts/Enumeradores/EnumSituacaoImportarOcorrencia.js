var EnumSituacaoImportarOcorrenciaHelper = function () {
    this.Todas = 0;
    this.AgIntegracao = 1;
    this.Falha = 2;
    this.Finalizada = 3;
};

EnumSituacaoImportarOcorrenciaHelper.prototype = {
    obterOpcoesPesquisa: function () {
        var situacoes = [];

        situacoes.push({ text: "Todas", value: this.Todas });
        situacoes.push({ text: "Ag. Integração", value: this.AgIntegracao });
        situacoes.push({ text: "Falha na integração", value: this.Falha });
        situacoes.push({ text: "Finalizada", value: this.Finalizada });

        return situacoes;
    },
    obterOpcoes: function () {
        var situacoes = [];
        
        situacoes.push({ text: "Ag. Integração", value: this.AgIntegracao });
        situacoes.push({ text: "Falha na integração", value: this.Falha });
        situacoes.push({ text: "Finalizada", value: this.Finalizada });

        return situacoes;
    }
};

var EnumSituacaoImportarOcorrencia = Object.freeze(new EnumSituacaoImportarOcorrenciaHelper());