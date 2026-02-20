var EnumModelosOutrosDocumentosHelper = function () {
    this.Declaracao = 00;
    this.Outros = 99;
};

EnumModelosOutrosDocumentosHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.ModelosOutrosDocumentos.Declaracao, value: this.Declaracao },
            { text: Localization.Resources.Enumeradores.ModelosOutrosDocumentos.Outros, value: this.Outros }
        ];
    },
};

var EnumModelosOutrosDocumentos = Object.freeze(new EnumModelosOutrosDocumentosHelper());
