let EnumSituacaoApuracaoBonificacaoHelper = function () {
    this.Todas = "";
    this.AguardandoGeracaoOcorrencia = 1;
    this.Cancelado = 2;
    this.Finalizado = 3;
};

EnumSituacaoApuracaoBonificacaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aguardando Geração de Ocorrência", value: this.AguardandoGeracaoOcorrencia },
            { text: "Cancelado", value: this.Cancelado },
            { text: "Finalizado", value: this.Finalizado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
};

let EnumSituacaoApuracaoBonificacao = Object.freeze(new EnumSituacaoApuracaoBonificacaoHelper());