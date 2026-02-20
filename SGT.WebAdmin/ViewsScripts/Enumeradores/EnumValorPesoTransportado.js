var EnumValorPesoTransportadoHelper = function () {
    this.ValorFixo = 1;
    this.Multiplicacao = 2;

};

EnumValorPesoTransportadoHelper.prototype = {
    ObterOpcoes: function (opcaoSelecione) {
        var arrayOpcoes = [
            { text: Localization.Resources.Enumeradores.ValorPesoTransportadoTabelaFrete.ValorFixo, value: this.ValorFixo },
            { text: Localization.Resources.Enumeradores.ValorPesoTransportadoTabelaFrete.Multiplicacao, value: this.Multiplicacao },
        ];
        return arrayOpcoes;
    }
};

var EnumValorPesoTransportado = Object.freeze(new EnumValorPesoTransportadoHelper());