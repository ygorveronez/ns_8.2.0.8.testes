var EnumTipoCalculoVolumeTabelaFreteComponenteFreteHelper = function () {
    this.PorFracao = 0;
    this.PorUnidadeIncompleta = 1;
    this.PorUnidadeCompleta = 2;
    this.PorValorFixoComExcedente = 3;
    this.PorFracaoExcedentePesoMinimo = 4;
};

EnumTipoCalculoVolumeTabelaFreteComponenteFreteHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoCalculoPeso.PorFracao, value: this.PorFracao },
            { text: Localization.Resources.Enumeradores.TipoCalculoPeso.PorUnidadeCompleta, value: this.PorUnidadeCompleta },
            { text: Localization.Resources.Enumeradores.TipoCalculoPeso.PorUnidadeIncompleta, value: this.PorUnidadeIncompleta },
            { text: Localization.Resources.Enumeradores.TipoCalculoPeso.PorValorFixoExcedente, value: this.PorValorFixoComExcedente },
            { text: Localization.Resources.Enumeradores.TipoCalculoPeso.PorFracaoSomenteExcedente, value: this.PorFracaoExcedentePesoMinimo }
        ];
    }
};

var EnumTipoCalculoVolumeTabelaFreteComponenteFrete = Object.freeze(new EnumTipoCalculoVolumeTabelaFreteComponenteFreteHelper());
