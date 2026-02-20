var EnumAguardandoIntegracaoHelper = function () {
    this.Todos = "";
    this.Confirmado = 0;
    this.Aguardando = 1;
};

EnumAguardandoIntegracaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Confirmado", value: this.Confirmado },
            { text: "Aguardando", value: this.Aguardando }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumAguardandoIntegracao = Object.freeze(new EnumAguardandoIntegracaoHelper());