var EnumIdiomaTemplateMensagemWhatsAppHelper = function () {
    this.Portugues = "pt_BR";
    this.Espanhol = "es_MX";
    this.Ingles = "en";
};

EnumIdiomaTemplateMensagemWhatsAppHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Português", value: this.Portugues },
            { text: "Espanhol", value: this.Espanhol },
            { text: "Inglês", value: this.Ingles }
        ];
    }
};

var EnumIdiomaTemplateMensagemWhatsApp = Object.freeze(new EnumIdiomaTemplateMensagemWhatsAppHelper());