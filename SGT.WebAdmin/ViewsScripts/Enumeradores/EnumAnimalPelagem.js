const EnumPelagemHelper = function () {
    this.Todas = 0;
    this.Curta = 1;
    this.Media = 2;
    this.Longa = 3;
};

EnumPelagemHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.Pelagem.Curta, value: this.Curta },
            { text: Localization.Resources.Enumeradores.Pelagem.Media, value: this.Media },
            { text: Localization.Resources.Enumeradores.Pelagem.Longa, value: this.Longa }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.Pelagem.Todas, value: this.Todas }].concat(this.obterOpcoes());
    },
};

const EnumPelagem = Object.freeze(new EnumPelagemHelper());
