var EnumTipoEmissorDocumentoHelper = function () {
    this.Integrador = 0;
    this.NSTech = 1;
};

EnumTipoEmissorDocumentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Integrador", value: this.Integrador },
            { text: "NSTech", value: this.NSTech }
        ];
    }
};

var EnumTipoEmissorDocumento = Object.freeze(new EnumTipoEmissorDocumentoHelper());
