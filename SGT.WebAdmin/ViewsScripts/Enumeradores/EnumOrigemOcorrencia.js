var EnumOrigemOcorrenciaHelper = function () {
    this.PorCarga = 1;
    this.PorPeriodo = 2;
    this.PorContrato = 3;
};

EnumOrigemOcorrenciaHelper.prototype = {
    obterOpcoes: function () {
        var opcoes = [];

        opcoes.push({ text: Localization.Resources.Enumeradores.OrigemOcorrencia.PorCarga, value: this.PorCarga });

        opcoes.push({ text: Localization.Resources.Enumeradores.OrigemOcorrencia.PorContrato, value: this.PorContrato });

        opcoes.push({ text: Localization.Resources.Enumeradores.OrigemOcorrencia.PorPeriodo, value: this.PorPeriodo });

        return opcoes
    },

}

var EnumOrigemOcorrencia = Object.freeze(new EnumOrigemOcorrenciaHelper());
