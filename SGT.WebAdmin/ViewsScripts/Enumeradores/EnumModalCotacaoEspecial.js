let EnumModalCotacaoEspecialHelper = function () {
    this.Todos = 0;
    this.Expresso = 1;
    this.Dedicado = 2;
};

EnumModalCotacaoEspecialHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Expresso", value: this.Expresso },
            { text: "Dedicado", value: this.Dedicado },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.SituacaoPedido.Todos, value: this.Todos }].concat(this.obterOpcoes());
    },
};

let EnumModalCotacaoEspecial = Object.freeze(new EnumModalCotacaoEspecialHelper());