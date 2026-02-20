var EnumEixosSuspensoHelper = function () {
    this.Todos = "";
    this.Ida = 1;
    this.Volta = 2;
    this.Nenhum = 3;
};

EnumEixosSuspensoHelper.prototype = {
    obterDescricao: function (tipo) {
        switch (tipo) {
            case this.Ida: return Localization.Resources.Enumeradores.EixosSuspenso.Ida;
            case this.Volta: return Localization.Resources.Enumeradores.EixosSuspenso.Volta;
            case this.Nenhum: return Localization.Resources.Enumeradores.EixosSuspenso.Nenhum;
            default: return "";
        }
    },
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.EixosSuspenso.Ida, value: this.Ida },
            { text: Localization.Resources.Enumeradores.EixosSuspenso.Volta, value: this.Volta },
            { text: Localization.Resources.Enumeradores.EixosSuspenso.Nenhum, value: this.Nenhum }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.EixosSuspenso.Todos, value: this.Todos }].concat(this.obterOpcoes());
    },
    obterDefault: function () {
        return [{ text: Localization.Resources.Enumeradores.EixosSuspenso.Volta, value: this.Volta }]
    }
};

var EnumEixosSuspenso = Object.freeze(new EnumEixosSuspensoHelper());
