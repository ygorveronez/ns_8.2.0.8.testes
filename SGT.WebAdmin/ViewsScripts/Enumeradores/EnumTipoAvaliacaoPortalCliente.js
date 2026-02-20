var EnumTipoAvaliacaoPortalClienteHelper = function () {
    this.Todos = "";
    this.Geral = 1;
    this.Individual = 2;
};

EnumTipoAvaliacaoPortalClienteHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Geral", value: this.Geral },
            { text: "Individual", value: this.Individual },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoAvaliacaoPortalCliente = Object.freeze(new EnumTipoAvaliacaoPortalClienteHelper());
