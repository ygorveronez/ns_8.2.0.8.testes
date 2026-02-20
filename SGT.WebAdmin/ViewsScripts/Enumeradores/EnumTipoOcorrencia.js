var EnumTipoOcorrenciaHelper = function () {
    this.Pendente = "P";
    this.Final = "F";
};

EnumTipoOcorrenciaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoOcorrencia.Pendente, value: this.Pendente },
            { text: Localization.Resources.Enumeradores.TipoOcorrencia.Final, value: this.Final }
        ];
    },
}

var EnumTipoOcorrencia = Object.freeze(new EnumTipoOcorrenciaHelper());
