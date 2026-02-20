var EnumTipoRateioHelper = function () {
    this.Padrao = 1;
    this.PorPeso = 2;
    this.PorPesoLiquido = 3;

};

EnumTipoRateioHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoRateio.Padrao, value: this.Padrao },
            { text: Localization.Resources.Enumeradores.TipoRateio.PorPeso, value: this.PorPeso },
            { text: Localization.Resources.Enumeradores.TipoRateio.PorPesoLiquido, value: this.PorPesoLiquido }
        ];
    },
}

var EnumTipoRateio = Object.freeze(new EnumTipoRateioHelper());