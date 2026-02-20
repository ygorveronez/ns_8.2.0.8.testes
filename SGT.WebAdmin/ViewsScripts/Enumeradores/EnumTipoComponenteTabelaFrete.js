var EnumTipoComponenteTabelaFreteHelper = function () {
    this.ValorFixo = 0;
    this.ValorCalculado = 1;
};

EnumTipoComponenteTabelaFreteHelper.prototype = {
    ObterOpcoes: function (opcaoSelecione) {
        var arrayOpcoes = [
            { text: Localization.Resources.Enumeradores.TipoComponenteTabelaFrete.ValorFixo, value: this.ValorFixo },
            { text: Localization.Resources.Enumeradores.TipoComponenteTabelaFrete.ValorCalculado, value: this.ValorCalculado },
        ];

        return arrayOpcoes;
    }
};

var EnumTipoComponenteTabelaFrete = Object.freeze(new EnumTipoComponenteTabelaFreteHelper());