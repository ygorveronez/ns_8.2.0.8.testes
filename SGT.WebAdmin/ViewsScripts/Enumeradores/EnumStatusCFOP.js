var EnumStatusCFOPHelper = function () {
    this.Inativo = "I";
    this.Ativo = "A";
};

EnumStatusCFOPHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Ativo", value: this.Ativo },
            { text: "Inativo", value: this.Inativo },
        ];
    },
    obterOpcoesPesquisa: function () {
        return this.obterOpcoes();
    },

};

var EnumStatusCFOP = Object.freeze(new EnumStatusCFOPHelper());