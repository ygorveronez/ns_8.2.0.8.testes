var EnumTipoCobrancaHoraTabelaFreteHelper = function () {
    this.PorFaixaHora = 1;
    this.ValorFixoPorHora = 2;
};

EnumTipoCobrancaHoraTabelaFreteHelper.prototype = {
    ObterOpcoes: function (opcaoSelecione) {
        var arrayOpcoes = [
            { text: Localization.Resources.Enumeradores.TipoCobrancaHoraTabelaFrete.PorFaixaHora, value: this.PorFaixaHora },
            { text: Localization.Resources.Enumeradores.TipoCobrancaHoraTabelaFrete.ValorFixoPorHora, value: this.ValorFixoPorHora },
        ];

        return arrayOpcoes;
    }
};

var EnumTipoCobrancaHoraTabelaFrete = Object.freeze(new EnumTipoCobrancaHoraTabelaFreteHelper());