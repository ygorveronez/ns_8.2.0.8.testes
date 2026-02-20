var EnumFinalidadeTipoOcorrenciaHelper = function () {
    this.Ambos = "";
    this.EDI = 1;
    this.Valor = 2;
    
};

EnumFinalidadeTipoOcorrenciaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.FinalidadeTipoOcorrencia.Ambos, value: this.Ambos },
            { text: Localization.Resources.Enumeradores.FinalidadeTipoOcorrencia.EDI, value: this.EDI },
            { text: Localization.Resources.Enumeradores.FinalidadeTipoOcorrencia.Valor, value: this.Valor }
        ];
    },
}

var EnumFinalidadeTipoOcorrencia = Object.freeze(new EnumFinalidadeTipoOcorrenciaHelper());