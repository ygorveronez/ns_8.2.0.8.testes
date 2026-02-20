var EnumSituacaoPagamentoMotoristaHelper = function () {
    this.Todas = 0;
    this.AgAprovacao = 1;
    this.Finalizada = 2;
    this.Rejeitada = 3;
    this.Cancelada = 4;
    this.AgIntegracao = 5;
    this.FalhaIntegracao = 6;
    this.AutorizacaoPendente = 7;
    this.AgInformacoes = 8;
    this.SemRegraAprovacao = 9;
    this.FinalizadoPagamento = 10;
};

EnumSituacaoPagamentoMotoristaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Ag. Aprovação", value: this.AgAprovacao },
            { text: "Finalizada", value: this.Finalizada },
            { text: "Rejeitada", value: this.Rejeitada },
            { text: "Cancelada", value: this.Cancelada },
            { text: "Ag. Integração", value: this.AgIntegracao },
            { text: "Falha na Integração", value: this.FalhaIntegracao },
            { text: "Autorização Pendente", value: this.AutorizacaoPendente },
            { text: "Ag. Informações", value: this.AgInformacoes },
            { text: "Sem Regra de Aprovação", value: this.SemRegraAprovacao },
            { text: "Finalizado Pagamento", value: this.FinalizadoPagamento }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoPagamentoMotorista = Object.freeze(new EnumSituacaoPagamentoMotoristaHelper());