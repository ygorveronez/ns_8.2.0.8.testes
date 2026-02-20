var EnumTipoOperadoraHelper = function () {
    this.Todos = "";
    this.Fixo = 1;
    this.Movel = 2;
};

EnumTipoOperadoraHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Fixo", value: this.Fixo },
            { text: "Móvel", value: this.Movel }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoOperadora = Object.freeze(new EnumTipoOperadoraHelper());
