const EnumSituacaoPagamentoHelper = function () {
    this.Todas = "";
    this.EmFechamento = 1;
    this.PendenciaFechamento = 2;
    this.AguardandoIntegracao = 3;
    this.EmIntegracao = 4;
    this.FalhaIntegracao = 5;
    this.Finalizado = 6;
    this.AguardandoAprovacao = 7;
    this.Reprovado = 8;
    this.SemRegraAprovacao = 9;
    this.Cancelado = 10;
};

EnumSituacaoPagamentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aguardando Aprovação", value: this.AguardandoAprovacao },
            { text: "Aguardando Integração", value: this.AguardandoIntegracao },
            { text: "Aprovação Rejeitada", value: this.Reprovado },
            { text: "Em Fechamento", value: this.EmFechamento },
            { text: "Em Integração", value: this.EmIntegracao },
            { text: "Falha na integração", value: this.FalhaIntegracao },
            { text: "Finalizado", value: this.Finalizado },
            { text: "Pendência no Fechamento", value: this.PendenciaFechamento },
            { text: "Sem Regra de Aprovação", value: this.SemRegraAprovacao },
            { text: "Cancelado", value: this.Cancelado },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
}

const EnumSituacaoPagamento = Object.freeze(new EnumSituacaoPagamentoHelper());