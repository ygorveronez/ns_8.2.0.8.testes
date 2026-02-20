var EnumTipoPropostaHelper = function () {
    this.Nenhum = 0;
    this.CargaFechada = 1;
    this.CargaFracionada = 2;
    this.Feeder = 3;
    this.VAS = 4;
};

EnumTipoPropostaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoPropostaOcorrencia.Nenhum, value: this.Nenhum },
            { text: Localization.Resources.Enumeradores.TipoPropostaOcorrencia.CargaFechada, value: this.CargaFechada },
            { text: Localization.Resources.Enumeradores.TipoPropostaOcorrencia.CargaFracionada, value: this.CargaFracionada },
            { text: Localization.Resources.Enumeradores.TipoPropostaOcorrencia.Feeder, value: this.Feeder },
            { text: Localization.Resources.Enumeradores.TipoPropostaOcorrencia.VAS, value: this.VAS }
        ];
    },
}

var EnumTipoProposta = Object.freeze(new EnumTipoPropostaHelper());