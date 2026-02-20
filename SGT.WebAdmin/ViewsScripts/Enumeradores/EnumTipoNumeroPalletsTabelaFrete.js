var EnumTipoNumeroPalletsTabelaFreteHelper = function () {
    this.PorFaixaPallets = 1;
    this.ValorFixoPorPallet = 2;

};

EnumTipoNumeroPalletsTabelaFreteHelper.prototype = {
    ObterOpcoes: function (opcaoSelecione) {
        var arrayOpcoes = [
            { text: Localization.Resources.Enumeradores.TipoPalletTabelaFrete.PorFaixaPallet, value: this.PorFaixaPallets },
            { text: Localization.Resources.Enumeradores.TipoPalletTabelaFrete.ValorFixoPallet, value: this.ValorFixoPorPallet },
        ];

        return arrayOpcoes;
    }
};

var EnumTipoNumeroPalletsTabelaFrete = Object.freeze(new EnumTipoNumeroPalletsTabelaFreteHelper());