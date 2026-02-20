var EnumSituacaoContratoFreteHelper = function () {
    this.Todos = 0;
    this.Aprovado = 1;
    this.AgAprovacao = 2;
    this.Rejeitado = 3;
    this.Finalizada = 4;
    this.Cancelado = 5;
    this.SemRegra = 6;
    this.Aberto = 7;
};

EnumSituacaoContratoFreteHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { value: this.Todos, text: "Todos" },
            { value: this.Aberto, text: "Aberto" },
            { value: this.AgAprovacao, text: "Ag. Aprovação" },
            { value: this.Aprovado, text: "Aprovado" },
            { value: this.Finalizada, text: "Finalizado" },
            { value: this.Rejeitado, text: "Rejeitado" },
            { value: this.SemRegra, text: "Sem Regra" },
            { value: this.Cancelado, text: "Cancelado" }
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.ObterOpcoes());
    }
};

var EnumSituacaoContratoFrete = Object.freeze(new EnumSituacaoContratoFreteHelper());