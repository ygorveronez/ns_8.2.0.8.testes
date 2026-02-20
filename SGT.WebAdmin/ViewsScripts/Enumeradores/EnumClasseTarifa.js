var EnumClasseTarifaHelper = function () {
    this.Todos = "";
    this.Minima = "M";
    this.Geral = "G";
    this.Especifica = "E";
};

EnumClasseTarifaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.ClasseTarifa.TarifaMinima, value: this.Minima },
            { text: Localization.Resources.Enumeradores.ClasseTarifa.TarifaGeral, value: this.Geral },
            { text: Localization.Resources.Enumeradores.ClasseTarifa.TarifaEspecifica, value: this.Especifica }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.ClasseTarifa.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumClasseTarifa = Object.freeze(new EnumClasseTarifaHelper());