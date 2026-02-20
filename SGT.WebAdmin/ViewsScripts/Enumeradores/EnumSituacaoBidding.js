const EnumSituacaoBiddingHelper = function () {
    this.Todas = "";    
    this.Finalizada = 0;
    this.Cancelada = 1;
    this.SemRegraAprovacao = 2;
    this.AguardandoAprovacao = 3;
    this.AprovacaoRejeitada = 4;
};

EnumSituacaoBiddingHelper.prototype = {
    ObterOpcoes: function () {
        return [            
            { text: "Finalizada", value: this.Finalizada },
            { text: "Cancelada", value: this.Cancelada },
            { text: "Sem Regra Aprovação", value: this.SemRegraAprovacao },
            { text: "Aguardando Aprovação", value: this.AguardandoAprovacao },
            { text: "Aprovação Rejeitada", value: this.AprovacaoRejeitada }
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.ObterOpcoes());
    }
};

const EnumSituacaoBidding = Object.freeze(new EnumSituacaoBiddingHelper());