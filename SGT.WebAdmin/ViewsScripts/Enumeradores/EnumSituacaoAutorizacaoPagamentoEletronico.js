var EnumSituacaoAutorizacaoPagamentoEletronicoHelper = function () {
    this.Todos = 0;
    this.Iniciada = 1;
    this.Finalizada = 2;
    this.SemRegraAprovacao = 3;
    this.AguardandoAprovacao = 4;
    this.AprovacaoRejeitada = 5;
};

EnumSituacaoAutorizacaoPagamentoEletronicoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Iniciada", value: this.Iniciada },
            { text: "Finalizada", value: this.Finalizada },
            { text: "Sem Regra", value: this.SemRegraAprovacao },
            { text: "Aguardando Aprovação", value: this.AguardandoAprovacao },
            { text: "Rejeitada", value: this.AprovacaoRejeitada }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoAutorizacaoPagamentoEletronico = Object.freeze(new EnumSituacaoAutorizacaoPagamentoEletronicoHelper());