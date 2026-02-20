var EnumSituacaoLoteClienteHelper = function () {
    this.EmCriacao = 1;
    this.AgIntegracao = 2;
    this.FalhaIntegracao = 3;
    this.Finalizado = 4;
};

EnumSituacaoLoteClienteHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: "Em Criação", value: this.EmCriacao },
            { text: "Ag. Integração", value: this.AgIntegracao },
            { text: "Falha na Integração", value: this.FalhaIntegracao },
            { text: "Finalizado", value: this.Finalizado }
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: "" }].concat(this.ObterOpcoes());
    }
};

var EnumSituacaoLoteCliente = Object.freeze(new EnumSituacaoLoteClienteHelper());