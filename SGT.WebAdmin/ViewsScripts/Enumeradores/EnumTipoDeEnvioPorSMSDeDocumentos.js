var EnumTipoDeEnvioPorSMSDeDocumentosHelper = function () {
    this.Nenhum = 0;
    this.SMS = 1;
    this.WhatsApp = 2;
};

EnumTipoDeEnvioPorSMSDeDocumentosHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Nenhum", value: this.Nenhum },
            { text: "SMS", value: this.SMS },
        ];
    }
};

var EnumTipoDeEnvioPorSMSDeDocumentos = Object.freeze(new EnumTipoDeEnvioPorSMSDeDocumentosHelper());