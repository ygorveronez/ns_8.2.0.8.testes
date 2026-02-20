var EnumSemanaHelper = function () {
    this.Todas = "";
    this.Primeira = 0;
    this.Segunda = 1;
    this.Terceira = 2;
    this.Quarta = 3;
};

EnumSemanaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.Semana.Primeira, value: this.Primeira },
            { text: Localization.Resources.Enumeradores.Semana.Segunda, value: this.Segunda },
            { text: Localization.Resources.Enumeradores.Semana.Terceira, value: this.Terceira },
            { text: Localization.Resources.Enumeradores.Semana.Quarta, value: this.Quarta }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.Semana.Todas, value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumSemana = Object.freeze(new EnumSemanaHelper());