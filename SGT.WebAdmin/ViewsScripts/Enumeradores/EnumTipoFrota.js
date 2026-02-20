var EnumTipoFrotaHelper = function () {
    this.NaoDefinido = 0;
    this.Fixo = 1;
    this.Spot = 2;
    this.Projeto = 3;
};

EnumTipoFrotaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoFrota.Fixo, value: this.Fixo },
            { text: Localization.Resources.Enumeradores.TipoFrota.NaoDefinido, value: this.NaoDefinido },
            { text: Localization.Resources.Enumeradores.TipoFrota.Spot, value: this.Spot },
            { text: Localization.Resources.Enumeradores.TipoFrota.Projeto, value: this.Projeto },
        ];
    },
};

var EnumTipoFrota = Object.freeze(new EnumTipoFrotaHelper());