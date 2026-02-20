var EnumSituacaoAjusteTabelaFreteHerper = function () {
    this.Todas = 9;
    this.Pendente = 0;
    this.Finalizado = 1;
    this.Cancelado = 2;
    this.AgAprovacao = 3;
    this.AgIntegracao = 5;
    this.SemRegraAprovacao = 4;
    this.RejeitadaAutorizacao = 6;
    this.EmProcessamento = 7;
    this.ProblemaProcessamento = 8;
    this.EmCriacao = 10;
    this.ProblemaCriacao = 11;
    this.EmAjuste = 12;
    this.ProblemaAjuste = 13;
};

EnumSituacaoAjusteTabelaFreteHerper.prototype = {
    obterOpcoesPesquisaAjusteTabelaFrete: function () {
        return [
            { text: "Todas", value: this.Todas },
            { text: "Cancelado", value: this.Cancelado },
            { text: "Finalizado", value: this.Finalizado },
            { text: "Pendente", value: this.Pendente },
            { text: "Ag. Aprovação", value: this.AgAprovacao },
            { text: "Ag. Integração", value: this.AgIntegracao },
            { text: "Em Processamento", value: this.EmProcessamento },
            { text: "Em Criação", value: this.EmCriacao },
            { text: "Em Ajuste", value: this.EmAjuste }
        ];
    },
    obterOpcoesPesquisaConsultaReajusteTabelaFrete: function () {
        return [
            { text: "Todas", value: this.Todas },
            { text: "Ag. Aprovação", value: this.AgAprovacao },
            { text: "Ag. Integração", value: this.AgIntegracao },
            { text: "Em Processamento", value: this.EmProcessamento },
            { text: "Finalizado", value: this.Finalizado },
            { text: "Problema no Processamento", value: this.ProblemaProcessamento },
            { text: "Rejeitada Autorização", value: this.RejeitadaAutorizacao },
            { text: "Sem Regra de Aprovação", value: this.SemRegraAprovacao }
        ];
    }
}

var EnumSituacaoAjusteTabelaFrete = Object.freeze(new EnumSituacaoAjusteTabelaFreteHerper());