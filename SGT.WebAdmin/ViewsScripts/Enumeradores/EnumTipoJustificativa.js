var EnumTipoJustificativaHelper = function () {
    this.Desconto = 1;
    this.Acrescimo = 2;
};

EnumTipoJustificativaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: 'Desconto', value: this.Desconto },
            { text: "Acréscimo", value: this.Acrescimo },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: "" }].concat(this.obterOpcoes());
    }
};

var EnumTipoJustificativa = Object.freeze(new EnumTipoJustificativaHelper());