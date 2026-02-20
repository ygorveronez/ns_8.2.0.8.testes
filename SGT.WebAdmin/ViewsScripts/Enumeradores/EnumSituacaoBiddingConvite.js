var EnumSituacaoBiddingConviteHelper = function () {
    this.Todas = "";
    this.Aguardando = 0;
    this.Checklist = 1;
    this.Ofertas = 2;
    this.Fechamento = 3;
    this.SemRegra = 4;
    this.AguardandoAprovacao = 5;
    this.AprovacaoRejeitada = 6;
};

EnumSituacaoBiddingConviteHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: "Aguardando", value: this.Aguardando },
            { text: "Checklist", value: this.Checklist },
            { text: "Ofertas", value: this.Ofertas },
            { text: "Fechamento", value: this.Fechamento },
            { text: "Sem Regra", value: this.SemRegra },
            { text: "Aguardando Aprovação", value: this.AguardandoAprovacao },
            { text: "Aprovação Rejeitada", value: this.AprovacaoRejeitada }
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.ObterOpcoes());
    }
};

var EnumSituacaoBiddingConvite = Object.freeze(new EnumSituacaoBiddingConviteHelper());