var EnumTipoMotoristaHelper = function () {
    this.Todos = 0;
    this.Proprio = 1;
    this.Terceiro = 2;
};

EnumTipoMotoristaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoMotorista.Proprio, value: this.Proprio },
            { text: Localization.Resources.Enumeradores.TipoMotorista.Terceiro, value: this.Terceiro },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoMotorista.Todos, value: this.Todos }].concat(this.obterOpcoes());
    },
};

var EnumTipoMotorista = Object.freeze(new EnumTipoMotoristaHelper());