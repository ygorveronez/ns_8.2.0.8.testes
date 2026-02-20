
var EnumTipoPesoTransportadoHelper = function () {
    this.PorFaixaPesoTransportado = 1;
    this.ValorFixoPorPesoTransportado = 2;

};

EnumTipoPesoTransportadoHelper.prototype = {
    ObterOpcoes: function (opcaoSelecione) {
        var arrayOpcoes = [
            { text: Localization.Resources.Enumeradores.TipoPesoTransportadoTabelaFrete.PorFaixaPesoTransportado, value: this.PorFaixaPesoTransportado },
            { text: Localization.Resources.Enumeradores.TipoPesoTransportadoTabelaFrete.ValorFixoPorPesoTransportado, value: this.ValorFixoPorPesoTransportado },
        ];

        return arrayOpcoes;
    }
};

var EnumTipoPesoTransportado = Object.freeze(new EnumTipoPesoTransportadoHelper());