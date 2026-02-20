var EnumTipoCobrancaAjudanteTabelaFreteHelper = function () {
    this.PorFaixaAjudantes = 1;
    this.ValorFixoPorAjudante = 2;

};

EnumTipoCobrancaAjudanteTabelaFreteHelper.prototype = {
    ObterOpcoes: function (opcaoSelecione) {
        var arrayOpcoes = [
            { text: Localization.Resources.Enumeradores.TipoCobrancaPorAjudante.PorFaixaAjudantes, value: this.PorFaixaAjudantes },
            { text: Localization.Resources.Enumeradores.TipoCobrancaPorAjudante.ValorFixoAjudante, value: this.ValorFixoPorAjudante },
        ];
        return arrayOpcoes;
    }
};

var EnumTipoCobrancaAjudanteTabelaFrete = Object.freeze(new EnumTipoCobrancaAjudanteTabelaFreteHelper());