var EnumTipoArredondamentoTabelaFreteHelper = function () {
    this.NaoArredondar = 0;
    this.ParaCima = 1;
    this.ParaBaixo = 2;
};

EnumTipoArredondamentoTabelaFreteHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoArredondamentoTabelaFrete.NaoArredondar, value: this.NaoArredondar },
            { text: Localization.Resources.Enumeradores.TipoArredondamentoTabelaFrete.ParaCima, value: this.ParaCima },
            { text: Localization.Resources.Enumeradores.TipoArredondamentoTabelaFrete.ParaBaixo, value: this.ParaBaixo }
        ];
    }
};

var EnumTipoArredondamentoTabelaFrete = Object.freeze(new EnumTipoArredondamentoTabelaFreteHelper());