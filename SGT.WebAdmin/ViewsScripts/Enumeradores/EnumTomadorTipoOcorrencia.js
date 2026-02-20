var EnumTomadorTipoOcorrenciaHelper = function () {
    this.IgualAoDocumentoAnterior = 1;
    this.Remetente = 2;
    this.Destinatario = 3;
    this.Outros = 4;
    this.Nenhum = 9;
};

EnumTomadorTipoOcorrenciaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TomadorTipoOcorrencia.IgualAoDocumentoAnterior, value: this.IgualAoDocumentoAnterior },
            { text: Localization.Resources.Enumeradores.TomadorTipoOcorrencia.Remetente, value: this.Remetente },
            { text: Localization.Resources.Enumeradores.TomadorTipoOcorrencia.Destinatario, value: this.Destinatario },
            { text: Localization.Resources.Enumeradores.TomadorTipoOcorrencia.Outros, value: this.Outros }
        ];
    },
}

var EnumTomadorTipoOcorrencia = Object.freeze(new EnumTomadorTipoOcorrenciaHelper());
