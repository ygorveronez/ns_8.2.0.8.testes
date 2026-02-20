

var EnumTipoCalculoComponenteTabelaFreteHelper = function () {
    this.Nenhum = 0;
    this.Percentual = 1;
    this.Peso = 2;
    this.QuantidadeDocumentos = 3;
    this.Tempo = 4;
    this.ValorFixo = 5;
    this.Eixo = 6;
    this.Cubagem = 7;
    this.ParametroFixo = 8;
    this.Volume = 9;
};

EnumTipoCalculoComponenteTabelaFreteHelper.prototype = {
    ObterOpcoes: function (opcaoSelecione) {
        var arrayOpcoes = [
            { text: Localization.Resources.Enumeradores.TipoCalculoComponenteFrete.Nenhum, value: this.Nenhum },
            { text: Localization.Resources.Enumeradores.TipoCalculoComponenteFrete.PorPercentual, value: this.Percentual },
            { text: Localization.Resources.Enumeradores.TipoCalculoComponenteFrete.PorPeso, value: this.Peso },
            { text: Localization.Resources.Enumeradores.TipoCalculoComponenteFrete.PorQuantidadeDocumentos, value: this.QuantidadeDocumentos },
            { text: Localization.Resources.Enumeradores.TipoCalculoComponenteFrete.PorTempo, value: this.Tempo },
            { text: Localization.Resources.Enumeradores.TipoCalculoComponenteFrete.ValorFixo, value: this.ValorFixo },
            { text: Localization.Resources.Enumeradores.TipoCalculoComponenteFrete.PorEixo, value: this.Eixo },
            { text: Localization.Resources.Enumeradores.TipoCalculoComponenteFrete.PorCubagem, value: this.Cubagem },
            { text: Localization.Resources.Enumeradores.TipoCalculoComponenteFrete.PorParametroFixo, value: this.ParametroFixo },
            { text: Localization.Resources.Enumeradores.TipoCalculoComponenteFrete.Volume, value: this.Volume },
        ];

        if (opcaoSelecione) {
            arrayOpcoes.push({ text: Localization.Resources.Enumeradores.TipoCalculoComponenteFrete.Nenhum, value: this.Nenhum });
        }

        return arrayOpcoes;
    }
};

var EnumTipoCalculoComponenteTabelaFrete = Object.freeze(new EnumTipoCalculoComponenteTabelaFreteHelper());