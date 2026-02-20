var EnumSituacaoContratoFreteTransportadorHelper = function () {
    this.Todos = 0;
    this.Aprovado = 1;
    this.AgAprovacao = 2;
    this.Rejeitado = 3;
    this.SemRegra = 4;
    this.Novo = 5;
    this.Vencido = 6;
    this.AgIntegracao = 7;
    this.ProblemaIntegracao = 8;
    this.Integrado = 9;
};

EnumSituacaoContratoFreteTransportadorHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: "Aprovado", value: this.Aprovado },
            { text: "Ag. Aprovação", value: this.AgAprovacao },
            { text: "Rejeitado", value: this.Rejeitado },
            { text: "Sem regra", value: this.SemRegra },
            { text: "Novo", value: this.Novo },
            { text: "Vencido", value: this.Vencido },
            { text: "Ag. Integração", value: this.AgIntegracao },
            { text: "Problema Integração", value: this.ProblemaIntegracao },
            { text: "Integrado", value: this.Integrado },
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.ObterOpcoes());
    }
};

var EnumSituacaoContratoFreteTransportador = Object.freeze(new EnumSituacaoContratoFreteTransportadorHelper());