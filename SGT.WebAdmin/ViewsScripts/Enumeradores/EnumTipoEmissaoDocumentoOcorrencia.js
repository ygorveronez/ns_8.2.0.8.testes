var EnumTipoEmissaoDocumentoOcorrenciaHelper = function () {
    this.Todos = 0;
    this.SomenteFilialEmissora = 1;
    this.SomenteSubcontratada = 2;
};

EnumTipoEmissaoDocumentoOcorrenciaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoEmissaoDocumentoOcorrencia.Todos, value: this.Todos },
            { text: Localization.Resources.Enumeradores.TipoEmissaoDocumentoOcorrencia.SomenteFilialEmissora, value: this.SomenteFilialEmissora },
            { text: Localization.Resources.Enumeradores.TipoEmissaoDocumentoOcorrencia.SomenteSubcontratada , value: this.SomenteSubcontratada }
        ];
    }
}

var EnumTipoEmissaoDocumentoOcorrencia = Object.freeze(new EnumTipoEmissaoDocumentoOcorrenciaHelper());