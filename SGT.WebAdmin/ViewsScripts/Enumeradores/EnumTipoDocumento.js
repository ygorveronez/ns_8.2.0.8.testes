var EnumTipoDocumentoHelper = function () {
    this.NFe = 55;
    this.CTe = 57;
    this.DCe = 100;
    this.Outros = 99;
};

EnumTipoDocumentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoDocumento.NFe, value: this.NFe },
            { text: Localization.Resources.Enumeradores.TipoDocumento.CTe, value: this.CTe },
            { text: Localization.Resources.Enumeradores.TipoDocumento.DCe, value: this.DCe },
            { text: Localization.Resources.Enumeradores.TipoDocumento.Outros, value: this.Outros }
        ];
    },
    obterOpcoesSemOutros: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoDocumento.NFe, value: this.NFe },
            { text: Localization.Resources.Enumeradores.TipoDocumento.CTe, value: this.CTe },
            { text: Localization.Resources.Enumeradores.TipoDocumento.DCe, value: this.DCe },
        ];
    },
    obterOpcoesLeituraDinamicaXML: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoDocumento.NFe, value: this.NFe }
        ];
    }
};

var EnumTipoDocumento = Object.freeze(new EnumTipoDocumentoHelper());