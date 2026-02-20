var EnumSituacaoAcompanhamentoArquivoHelper = function () {
    this.Todos = 0;
    this.AguardandoProcessamento = 1;
    this.ErroNoArquivo = 2;
    this.ErroNoProcessamento = 3;
    this.Processado = 4;
    this.Cancelado =5;
};

EnumSituacaoAcompanhamentoArquivoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aguardando Processamento", value: this.AguardandoProcessamento },
            { text: "Erro no Arquivo", value: this.ErroNoArquivo },
            { text: "Erro no Processamento", value: this.ErroNoProcessamento },
            { text: "Processado", value: this.Processado },
            { text: "Cancelado", value: this.Cancelado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoAcompanhamentoArquivo = Object.freeze(new EnumSituacaoAcompanhamentoArquivoHelper());