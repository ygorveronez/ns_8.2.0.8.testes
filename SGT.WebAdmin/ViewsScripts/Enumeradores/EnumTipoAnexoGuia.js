var EnumTipoAnexoGuiaHelper = function () {
    this.Guia = 1;
    this.Comprovante = 2;
};

EnumTipoAnexoGuiaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Guia", value: this.Guia },
            { text: "Comprovante", value: this.Comprovante }
        ];
    }
};

var EnumTipoAnexoGuia = Object.freeze(new EnumTipoAnexoGuiaHelper());