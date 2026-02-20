var EnumTipoDistanciaHelper = function () {
    this.PorFaixaDistanciaPercorrida = 1;
    this.ValorFixoPorDistanciaPercorrida = 2;
};

EnumTipoDistanciaHelper.prototype = {
    ObterOpcoes: function (opcaoSelecione) {
        var arrayOpcoes = [
            { text: Localization.Resources.Enumeradores.TipoDistancia.PorFaixaDistanciaPercorrida, value: this.PorFaixaDistanciaPercorrida },
            { text: Localization.Resources.Enumeradores.TipoDistancia.ValorFixoPorDistanciaPercorrida, value: this.ValorFixoPorDistanciaPercorrida },
        ];

        return arrayOpcoes;
    }
};

var EnumTipoDistancia = Object.freeze(new EnumTipoDistanciaHelper());