var EnumQuinzenaHelper = function () {
    this.Todas = "";
    this.Primeira = 0;
    this.Segunda = 1;
};

EnumQuinzenaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.Quinzena.Primeira, value: this.Primeira },
            { text: Localization.Resources.Enumeradores.Quinzena.Segunda, value: this.Segunda }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.Quinzena.Todas, value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumQuinzena = Object.freeze(new EnumQuinzenaHelper());