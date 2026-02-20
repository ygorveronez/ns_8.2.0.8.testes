var EnumOpcaoRotaHelper = function () {
    this.Todas = 0;
    this.PossuiRota = 1;
    this.NaoPossuiRota = 2;
};

EnumOpcaoRotaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.OpcaoRota.PossuiRota, value: this.PossuiRota },
            { text: Localization.Resources.Enumeradores.OpcaoRota.NaoPossuiRota, value: this.NaoPossuiRota }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.OpcaoRota.Todas, value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumOpcaoRota = Object.freeze(new EnumOpcaoRotaHelper());
