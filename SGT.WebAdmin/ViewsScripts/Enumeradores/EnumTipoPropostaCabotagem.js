var EnumTipoPropostaCabotagemHelper = function () {
    this.Todos = 0;
    this.Cabotagem = 1;
    this.Feeder = 2;
};

EnumTipoPropostaCabotagemHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoProposta.Todos, value: this.Todos },
            { text: Localization.Resources.Enumeradores.TipoProposta.Cabotagem, value: this.Cabotagem },
            { text: Localization.Resources.Enumeradores.TipoProposta.Feeder, value: this.Feeder }
        ];
    },
};

var EnumTipoPropostaCabotagem = Object.freeze(new EnumTipoPropostaCabotagemHelper());