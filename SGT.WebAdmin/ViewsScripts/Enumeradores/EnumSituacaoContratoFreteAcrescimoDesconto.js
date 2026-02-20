var EnumSituacaoContratoFreteAcrescimoDescontoHelper = function () {
    this.Todos = 0;
    this.Aprovado = 1;
    this.AgAprovacao = 2;
    this.Rejeitado = 3;
    this.Finalizado = 4;
    this.Cancelado = 5;
    this.SemRegra = 6;
    this.AgIntegracao = 7;
    this.FalhaIntegracao = 8;
    this.AplicacaoValorRejeitado = 9;
};

EnumSituacaoContratoFreteAcrescimoDescontoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { value: this.AgAprovacao, text: "Ag. Aprovação" },
            { value: this.Aprovado, text: "Aprovado" },
            { value: this.Finalizado, text: "Finalizado" },
            { value: this.Rejeitado, text: "Rejeitado" },
            { value: this.Cancelado, text: "Cancelado" },
            { value: this.SemRegra, text: "Sem Regra" },
            { value: this.AgIntegracao, text: "Ag. Integração" },
            { value: this.FalhaIntegracao, text: "Falha na Integração" },
            { value: this.AplicacaoValorRejeitado, text: "Aplicação Valor Rejeitado" }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoContratoFreteAcrescimoDesconto = Object.freeze(new EnumSituacaoContratoFreteAcrescimoDescontoHelper());