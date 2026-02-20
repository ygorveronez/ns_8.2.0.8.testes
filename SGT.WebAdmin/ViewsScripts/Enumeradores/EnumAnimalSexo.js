var EnumPetSexoHelper = function () {
    this.Todos = 0;
    this.Macho = 1;
    this.Femea = 2;
};

EnumPetSexoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.AnimalSexo.Macho, value: this.Macho },
            { text: Localization.Resources.Enumeradores.AnimalSexo.Femea, value: this.Femea }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todos }].concat(this.obterOpcoes());
    },
};

var EnumPetSexo = Object.freeze(new EnumPetSexoHelper());