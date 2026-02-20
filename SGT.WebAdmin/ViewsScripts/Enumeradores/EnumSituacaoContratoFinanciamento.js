var EnumSituacaoContratoFinanciamentoHelper = function () {
    this.Todos = "";
    this.Aberto = 1;
    this.Cancelado = 2;
    this.Finalizado = 3;
};

EnumSituacaoContratoFinanciamentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aberto", value: this.Aberto },
            { text: "Finalizado", value: this.Finalizado },
            { text: "Cancelado", value: this.Cancelado },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoContratoFinanciamento = Object.freeze(new EnumSituacaoContratoFinanciamentoHelper());