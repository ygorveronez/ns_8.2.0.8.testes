var EnumDezenaHelper = function () {
    this.Todas = "";
    this.Primeira = 0;
    this.Segunda = 1;
    this.Terceira = 2;
};

EnumDezenaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.Dezena.Primeira, value: this.Primeira },
            { text: Localization.Resources.Enumeradores.Dezena.Segunda, value: this.Segunda },
            { text: Localization.Resources.Enumeradores.Dezena.Terceira, value: this.Terceira }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.Dezena.Todas, value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumDezena = Object.freeze(new EnumDezenaHelper());