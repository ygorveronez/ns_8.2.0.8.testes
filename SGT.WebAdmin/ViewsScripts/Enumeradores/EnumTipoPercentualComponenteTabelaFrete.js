var EnumTipoPercentualComponenteTabelaFreteHelper = function () {
    this.SobreValorFrete = 0;
    this.SobreNotaFiscal = 1;
    this.SobreValorFreteEComponentes = 2;
    this.SobreValorNotasExcedenteValorMinimo = 3;
};

EnumTipoPercentualComponenteTabelaFreteHelper.prototype = {
    ObterOpcoes: function (opcaoSelecione) {
        var arrayOpcoes = [
            { text: Localization.Resources.Enumeradores.TipoPercentualComponenteTabelaFrete.SobreValorFrete, value: this.SobreValorFrete },
            { text: Localization.Resources.Enumeradores.TipoPercentualComponenteTabelaFrete.SobreNotaFiscal, value: this.SobreNotaFiscal },
            { text: Localization.Resources.Enumeradores.TipoPercentualComponenteTabelaFrete.SobreValorFreteEComponentes, value: this.SobreValorFreteEComponentes },
            { text: Localization.Resources.Enumeradores.TipoPercentualComponenteTabelaFrete.SobreValorNotasExcedenteValorMinimo, value: this.SobreValorNotasExcedenteValorMinimo },
        ];
        return arrayOpcoes;
    }
};

var EnumTipoPercentualComponenteTabelaFrete = Object.freeze(new EnumTipoPercentualComponenteTabelaFreteHelper());