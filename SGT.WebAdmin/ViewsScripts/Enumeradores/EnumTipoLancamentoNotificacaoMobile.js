var EnumTipoLancamentoNotificacaoMobileHelper = function () {
    this.Todos = "";
    this.Automatico = 1;
    this.Manual = 2;
}

EnumTipoLancamentoNotificacaoMobileHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Automático", value: this.Automatico },
            { text: "Manual", value: this.Manual }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoLancamentoNotificacaoMobile = Object.freeze(new EnumTipoLancamentoNotificacaoMobileHelper());