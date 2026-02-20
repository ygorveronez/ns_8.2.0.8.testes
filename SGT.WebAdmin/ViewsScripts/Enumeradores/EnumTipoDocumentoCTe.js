var EnumTipoDocumentoCTeHelper = function () {
    this.NFeNotaFiscalEletronica = 0;
    this.NotaFiscal = 1;
    this.OutrosDocumentos = 2;
};

EnumTipoDocumentoCTeHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoDocumentoCTe.NFeNotaFiscalEletronica, value: this.NFeNotaFiscalEletronica },
            { text: Localization.Resources.Enumeradores.TipoDocumentoCTe.NotaFiscal, value: this.NotaFiscal },
            { text: Localization.Resources.Enumeradores.TipoDocumentoCTe.OutrosDocumentos, value: this.OutrosDocumentos },
        ];
    },

};

var EnumTipoDocumentoCTe = Object.freeze(new EnumTipoDocumentoCTeHelper());