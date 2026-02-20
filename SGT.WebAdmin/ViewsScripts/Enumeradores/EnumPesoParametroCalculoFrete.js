var EnumPesoParametroCalculoFreteHelper = function () {
    this.PesoDaCarga = 0;
    this.CapacidadeMinimaGarantidaModeloVeicular = 1;
    this.CapacidadeModeloVeicular = 2;
    this.ProporcionalCapacidadeModeloVeicular = 3;

};

EnumPesoParametroCalculoFreteHelper.prototype = {
    ObterOpcoes: function (opcaoSelecione) {
        var arrayOpcoes = [
            { text: Localization.Resources.Enumeradores.PesoParametroCalculoFreteTabelaFrete.PesoDaCarga, value: this.PesoDaCarga },
            { text: Localization.Resources.Enumeradores.PesoParametroCalculoFreteTabelaFrete.CapacidadeMinimaGarantidaModeloVeicular, value: this.CapacidadeMinimaGarantidaModeloVeicular },
            { text: Localization.Resources.Enumeradores.PesoParametroCalculoFreteTabelaFrete.CapacidadeModeloVeicular, value: this.CapacidadeModeloVeicular },
            { text: Localization.Resources.Enumeradores.PesoParametroCalculoFreteTabelaFrete.ProporcionalCapacidadeModeloVeicular, value: this.ProporcionalCapacidadeModeloVeicular },
        ];
        return arrayOpcoes;
    }
};

var EnumPesoParametroCalculoFrete = Object.freeze(new EnumPesoParametroCalculoFreteHelper());

