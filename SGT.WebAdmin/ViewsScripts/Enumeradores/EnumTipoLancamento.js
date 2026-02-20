var EnumTipoLancamentoHelper = function () {
    this.Todos = "";
    this.Automatico = 0;
    this.Manual = 1;
}

EnumTipoLancamentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Automatico", value: this.Automatico },
            { text: "Manual", value: this.Manual }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    },
};

var EnumTipoLancamento = Object.freeze(new EnumTipoLancamentoHelper());
