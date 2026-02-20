var EnumTipoServidorEmailHelper = function () {
    this.Outlook = 0;
    this.Gmail = 1;
    this.Outro = 2;
};

EnumTipoServidorEmailHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Outlook", value: this.Outlook },
            { text: 'Gmail', value: this.Gmail },
            { text: 'Outro', value: this.Outro }
        ];
    }
};

var EnumTipoServidorEmail = Object.freeze(new EnumTipoServidorEmailHelper());
