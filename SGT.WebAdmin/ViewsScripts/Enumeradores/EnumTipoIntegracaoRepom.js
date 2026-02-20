var EnumTipoIntegracaoRepomHelper = function () {
    this.SOAP = 0;
    this.REsT = 1;
};

EnumTipoIntegracaoRepomHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "SOAP", value: this.SOAP },
            { text: "REsT", value: this.REsT }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [
            { text: "SOAP", value: this.SOAP },
            { text: "REsT", value: this.REsT }
        ];
    }
};

var EnumTipoIntegracaoRepom = Object.freeze(new EnumTipoIntegracaoRepomHelper());