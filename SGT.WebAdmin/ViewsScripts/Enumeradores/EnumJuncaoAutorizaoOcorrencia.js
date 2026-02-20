var EnumJuncaoAutorizaoOcorrenciaHelper = function () {
    this.E = 1;
    this.Ou = 2;
};

EnumJuncaoAutorizaoOcorrenciaHelper.prototype = {
    obterOpcoes: function () {

        return [
            { text: Localization.Resources.Enumeradores.JuncaoAutorizaoOcorrencia.ETodasVerdadeiras, value: this.E },
            { text: Localization.Resources.Enumeradores.JuncaoAutorizaoOcorrencia.OuApenasUmaVerdadeira, value: this.Ou },
        ];
    },
};

var EnumJuncaoAutorizaoOcorrencia = Object.freeze(new EnumJuncaoAutorizaoOcorrenciaHelper());