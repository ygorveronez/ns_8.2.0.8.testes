var EnumSituacaoFluxoAvariaHelper = function () {
    this.Todas = 0;
    this.Dados = 1;
    this.Produtos = 2;
    this.AgAprovacao = 3;
    this.Termo = 4;
    this.AgLote = 5;
    this.LoteGerado = 6;
    this.AgIntegracao = 7;
    this.Destinacao = 8;
    this.Finalizado = 9;
    this.RejeitadaAutorizacao = 10;
    this.RejeitadaLote = 11;
    this.RejeitadaIntegracao = 12;
    this.SemRegraAprovacao = 13;
    this.SemRegraLote = 14;
    this.Cancelado = 15;
};

EnumSituacaoFluxoAvariaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Dados", value: this.Dados },
            { text: "Produtos", value: this.Produtos },
            { text: "Aguardando Aprovação", value: this.AgAprovacao },
            { text: "Termo", value: this.Termo },
            { text: "Aguardando Lote", value: this.AgLote },
            { text: "Lote Gerado", value: this.LoteGerado },
            { text: "Aguardando Integração", value: this.LoteGerado },
            { text: "Destinação", value: this.Destinacao },
            { text: "Finalizado", value: this.Finalizado },
            { text: "Autorização Rejeitada", value: this.RejeitadaAutorizacao },
            { text: "Lote Rejeitado", value: this.RejeitadaLote },
            { text: "Integração Rejeitada", value: this.RejeitadaIntegracao },
            { text: "Sem Regra de Aprovacao", value: this.SemRegraAprovacao },
            { text: "Sem Regra de Lote", value: this.SemRegraLote },
            { text: "Cancelado", value: this.Cancelado },
            { text: "Todas", value: this.Todas }
        ];
    }
}

var EnumSituacaoFluxoAvaria = Object.freeze(new EnumSituacaoFluxoAvariaHelper());