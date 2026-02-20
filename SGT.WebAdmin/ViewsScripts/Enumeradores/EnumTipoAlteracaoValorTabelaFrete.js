var EnumTipoAlteracaoValorTabelaFreteHelper = function () {
    this.paraQualquerValor = 0;
    this.somenteParaValorMaior = 1;
    this.somenteParaValorMenor = 2;

};

EnumTipoAlteracaoValorTabelaFreteHelper.prototype = {
    obterOpcoes: function (opcaoSelecione) {
        var arrayOpcoes = [
            { text: Localization.Resources.Enumeradores.TipoAlteracaoValorTabelaFrete.paraQualquerValor, value: this.paraQualquerValor },
            { text: Localization.Resources.Enumeradores.TipoAlteracaoValorTabelaFrete.somenteParaValorMaior, value: this.somenteParaValorMaior },
            { text: Localization.Resources.Enumeradores.TipoAlteracaoValorTabelaFrete.somenteParaValorMenor, value: this.somenteParaValorMenor },
        ];
        return arrayOpcoes;
    }
};

var EnumTipoAlteracaoValorTabelaFrete = Object.freeze(new EnumTipoAlteracaoValorTabelaFreteHelper());