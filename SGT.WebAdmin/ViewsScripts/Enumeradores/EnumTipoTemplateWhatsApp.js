var EnumTipoTemplateWhatsAppHelper = function () {
    this.AcompanhamentoEntrega = 1;
};

EnumTipoTemplateWhatsAppHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Acompanhamento Entrega", value: this.AcompanhamentoEntrega },
        ];
    }
};

var EnumTipoTemplateWhatsApp = Object.freeze(new EnumTipoTemplateWhatsAppHelper());