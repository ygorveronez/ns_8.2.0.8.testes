var EnumIndicadorIETomadorHelper = function () {
    this.Contribuinte = 1;
    this.Isento = 2;
    this.NaoContribuinte = 9;
};

EnumIndicadorIETomadorHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.InficadorIETomador.Contribuinte, value: this.Contribuinte },
            { text: Localization.Resources.Enumeradores.InficadorIETomador.Isento, value: this.Isento },
            { text: Localization.Resources.Enumeradores.InficadorIETomador.NaoContribuinte, value: this.NaoContribuinte },
        ];
    },

};

var EnumIndicadorIETomador = Object.freeze(new EnumIndicadorIETomadorHelper());