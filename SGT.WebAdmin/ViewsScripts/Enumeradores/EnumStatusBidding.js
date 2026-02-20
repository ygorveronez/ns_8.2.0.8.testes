var EnumStatusBiddingHelper = function () {
    this.Aguardando = 0;
    this.Checklist = 1;
    this.Ofertas = 2;
    this.Fechamento = 3;
    this.SemRegra = 4;
    this.AguardandoAprovacao = 5;
    this.AprovacaoRejeitada = 6;

}
EnumStatusBiddingHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aguardando Convite", value: this.Aguardando },
            { text: "Aguardando Checklist", value: this.Checklist },
            { text: "Aguardando Ofertas", value: this.Ofertas },
            { text: "Aguardando Aprovacao", value: this.AguardandoAprovacao },
            { text: "Aprovacao Rejeitada", value: this.AprovacaoRejeitada },
            { text: "Finalizado", value: this.Fechamento },
            { text: "Sem Regra", value: this.SemRegra },
        ];
    },
}

var EnumStatusBidding = Object.freeze(new EnumStatusBiddingHelper());
