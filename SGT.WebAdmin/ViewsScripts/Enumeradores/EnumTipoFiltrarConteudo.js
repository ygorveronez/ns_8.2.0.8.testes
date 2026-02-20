var EnumTipoFiltrarConteudoHelper = function () {
    this.TextoLivre = 1;
    this.ExpressaoRegular = 2;
};

EnumTipoFiltrarConteudoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Texto Livre", value: this.TextoLivre },
            { text: "Expressão Regular", value: this.ExpressaoRegular }
        ];
    }
};

var EnumTipoFiltrarConteudo = Object.freeze(new EnumTipoFiltrarConteudoHelper());