var EnumCorRacaHelper = function () {
    this.SemInformacao = 0;
    this.Branca = 1;
    this.Preta = 2;
    this.Parda = 3;
    this.Amarela = 4;
    this.Indigena = 5;
};

EnumCorRacaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.CorRaca.SemInformacao, value: this.SemInformacao },
            { text: Localization.Resources.Enumeradores.CorRaca.Branca, value: this.Branca },
            { text: Localization.Resources.Enumeradores.CorRaca.Preta, value: this.Preta },
            { text: Localization.Resources.Enumeradores.CorRaca.Parda, value: this.Parda },
            { text: Localization.Resources.Enumeradores.CorRaca.Amarela, value: this.Amarela },
            { text: Localization.Resources.Enumeradores.CorRaca.Indigena, value: this.Indigena },
        ];
    }
};

var EnumCorRaca = Object.freeze(new EnumCorRacaHelper());