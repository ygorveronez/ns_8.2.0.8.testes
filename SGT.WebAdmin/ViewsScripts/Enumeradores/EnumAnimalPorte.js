const EnumPorteHelper = function () {
    this.Todos = 0;
    this.Pequeno = 1;
    this.Medio = 2;
    this.Grande = 3;
};

EnumPorteHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.Porte.Pequeno, value: this.Pequeno },
            { text: Localization.Resources.Enumeradores.Porte.Medio, value: this.Medio },
            { text: Localization.Resources.Enumeradores.Porte.Grande, value: this.Grande }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todos }].concat(this.obterOpcoes());
    },
};

const EnumPorte = Object.freeze(new EnumPorteHelper());
