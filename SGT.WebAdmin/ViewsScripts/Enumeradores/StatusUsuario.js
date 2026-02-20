const EnumStatusUsuarioHelper = function () {
    this.Todas = "";
    this.Inativo = "I";
    this.Ativo = "A";
};

EnumStatusUsuarioHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Gerais.Geral.Ativo, value: this.Ativo },
            { text: Localization.Resources.Gerais.Geral.Inativo, value: this.Inativo },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    },

};

const EnumStatusUsuario = Object.freeze(new EnumStatusUsuarioHelper());