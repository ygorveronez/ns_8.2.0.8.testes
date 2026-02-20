var EnumSituacaoProcessamentoIntegracaoSuperAppHelper = function () {
    this.Todos = "";
    this.AguardandoProcessamento = 0;
    this.Processado = 1;
    this.ErroProcessamento = 2;
};

EnumSituacaoProcessamentoIntegracaoSuperAppHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Todos", value: this.Todos },
            { text: "Pendente Processamento", value: this.AguardandoProcessamento },
            { text: "Processados", value: this.Processado },
            { text: "Erro Processamento", value: this.ErroProcessamento }
        ];
    },
}

var EnumSituacaoProcessamentoIntegracaoSuperApp = Object.freeze(new EnumSituacaoProcessamentoIntegracaoSuperAppHelper());