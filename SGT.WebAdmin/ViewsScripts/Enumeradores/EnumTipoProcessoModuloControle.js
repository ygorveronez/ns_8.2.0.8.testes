var EnumTipoProcessoModuloControleHelper = function () {
    this.Todos = "";
    this.Automatico = 1;
    this.Manual = 2;
};

EnumTipoProcessoModuloControleHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Automático", value: this.Automatico },
            { text: "Manual", value: this.Manual }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todos }].concat(this.obterOpcoes());
    },
}

var EnumTipoProcessoModuloControle = Object.freeze(new EnumTipoProcessoModuloControleHelper());