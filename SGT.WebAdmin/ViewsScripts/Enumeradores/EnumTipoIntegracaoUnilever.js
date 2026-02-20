var EnumTipoIntegracaoUnileverHelper = function () {
    this.OTM = 1;
    this.None = 2;
    this.OtmSap = 3;
};

EnumTipoIntegracaoUnileverHelper.prototype = {
    obterOpcoes: function (opcaoSelecione) {
        var arrayOpcoes = [
            { text: 'OTM', value: this.OTM },
            { text: "None", value: this.None },
            { text: "OTM+SAP", value: this.OtmSap },
        ];

        return arrayOpcoes;
    }
};

var EnumTipoIntegracaoUnilever = Object.freeze(new EnumTipoIntegracaoUnileverHelper());