var EnumSituacaoLicitacaoParticipacaoHelper = function () {
    this.Todas = "";
    this.AguardandoOferta = 1;
    this.AguardandoRetornoOferta = 2;
    this.Cancelada = 3;
    this.OfertaAceita = 4;
    this.OfertaRecusada = 5;
}

EnumSituacaoLicitacaoParticipacaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aguardando Oferta", value: this.AguardandoOferta },
            { text: "Aguardando Retorno da Oferta", value: this.AguardandoRetornoOferta },
            { text: "Cancelada", value: this.Cancelada },
            { text: "Oferta Aceita", value: this.OfertaAceita },
            { text: "Oferta Recusada", value: this.OfertaRecusada }
        ];
    },
    obterOpcoesAprovacao: function () {
        return [
            { text: "Aguardando Retorno da Oferta", value: this.AguardandoRetornoOferta },
            { text: "Oferta Aceita", value: this.OfertaAceita },
            { text: "Oferta Recusada", value: this.OfertaRecusada }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    },
    obterOpcoesPesquisaAprovacao: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoesAprovacao());
    }
}

var EnumSituacaoLicitacaoParticipacao = Object.freeze(new EnumSituacaoLicitacaoParticipacaoHelper());