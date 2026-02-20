var EnumTipoFreeTimeHelper = function () {
    this.PorParada = 1;
    this.AcumulativoPorViagem = 2;
};

EnumTipoFreeTimeHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoFreeTime.PorParada, value: this.PorParada },
            { text: Localization.Resources.Enumeradores.TipoFreeTime.AcumulativoPorViagem, value: this.AcumulativoPorViagem }
        ];
    },
};

var EnumTipoFreeTime = Object.freeze(new EnumTipoFreeTimeHelper());