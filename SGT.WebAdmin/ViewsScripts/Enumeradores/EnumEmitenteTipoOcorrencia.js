var EnumEmitenteTipoOcorrenciaHelper = function () {
    this.IgualAoDocumentoAnterior = 1;
    this.Outros = 2;
    this.Nenhum = 9;
};

EnumEmitenteTipoOcorrenciaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.EmitenteTipoOcorrencia.IgualAoDocumentoAnterior, value: this.IgualAoDocumentoAnterior },
            { text: Localization.Resources.Enumeradores.EmitenteTipoOcorrencia.Outros, value: this.Outros }
        ];
    },
}

var EnumEmitenteTipoOcorrencia = Object.freeze(new EnumEmitenteTipoOcorrenciaHelper());
