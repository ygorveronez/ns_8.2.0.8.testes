var EnumValorAutomatizacaoTipoCargaValorHelper = function () {
    this.Ate = 0;
    this.AcimaDe = 1;
};

EnumValorAutomatizacaoTipoCargaValorHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.ValorAutomatizacaoTipoCarga.Ate, value: this.Ate },
            { text: Localization.Resources.Enumeradores.ValorAutomatizacaoTipoCarga.AcimaDe, value: this.AcimaDe },
        ];
    }
};

var EnumValorAutomatizacaoTipoCargaValor = Object.freeze(new EnumValorAutomatizacaoTipoCargaValorHelper());