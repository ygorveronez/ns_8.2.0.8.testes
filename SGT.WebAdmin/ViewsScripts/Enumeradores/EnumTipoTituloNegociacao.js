var EnumTipoTituloNegociacaoHelper = function () {
    this.Todos = 0;
    this.Originais = 1;
    this.Negociacao = 2;
};

EnumTipoTituloNegociacaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Originais", value: this.Originais },
            { text: "Gerados de Negociação", value: this.Negociacao }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoTituloNegociacao = Object.freeze(new EnumTipoTituloNegociacaoHelper());