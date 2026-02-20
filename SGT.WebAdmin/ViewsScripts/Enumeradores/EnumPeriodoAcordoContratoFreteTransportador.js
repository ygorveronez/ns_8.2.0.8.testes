var EnumPeriodoAcordoContratoFreteTransportadorHelper = function () {
    this.Semanal = 7;
    this.Decendial = 10;
    this.Quinzenal = 15;
    this.Mensal = 30;
    this.NaoPossui = 100;
};

EnumPeriodoAcordoContratoFreteTransportadorHelper.prototype = {
    obterOpcoes: function (integracaoLBC) {
        var opcoes = ([
            { text: Localization.Resources.Enumeradores.PeriodoAcordoContratoFreteTransportador.Semanal, value: this.Semanal },
            { text: Localization.Resources.Enumeradores.PeriodoAcordoContratoFreteTransportador.Decendial, value: this.Decendial },
            { text: Localization.Resources.Enumeradores.PeriodoAcordoContratoFreteTransportador.Quinzenal, value: this.Quinzenal },
            { text: Localization.Resources.Enumeradores.PeriodoAcordoContratoFreteTransportador.Mensal, value: this.Mensal }
        ]);

        if (integracaoLBC)
            return [{ text: "Selecione uma opção", value: "" }].concat(opcoes);
        return opcoes;
    },
    obterOpcoesFranquia: function (integracaoLBC) {
        var opcoes = ([
            { text: Localization.Resources.Enumeradores.PeriodoAcordoContratoFreteTransportador.Semanal, value: this.Semanal },
            { text: Localization.Resources.Enumeradores.PeriodoAcordoContratoFreteTransportador.Decendial, value: this.Decendial },
            { text: Localization.Resources.Enumeradores.PeriodoAcordoContratoFreteTransportador.Quinzenal, value: this.Quinzenal },
            { text: Localization.Resources.Enumeradores.PeriodoAcordoContratoFreteTransportador.Mensal, value: this.Mensal },
            { text: Localization.Resources.Enumeradores.PeriodoAcordoContratoFreteTransportador.SemFranquia, value: this.NaoPossui }
        ]);

        if (integracaoLBC)
            return [{ text: "Selecione uma opção", value: "" }].concat(opcoes);
        return opcoes;
    }
}

var EnumPeriodoAcordoContratoFreteTransportador = Object.freeze(new EnumPeriodoAcordoContratoFreteTransportadorHelper());