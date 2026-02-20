let EnumVersaoLayoutATMHelper = function () {
    this.Versao200 = 1;
    this.Versao400 = 2;
};

EnumVersaoLayoutATMHelper.prototype = {
    obterOpcoes: function () {
        return [
            {
                value: this.Versao200,
                text: "Versão 2.00"
            }, {
                value: this.Versao400,
                text: "Versão 4.00"
            }
        ];
    }
}

let EnumVersaoLayoutATM = Object.freeze(new EnumVersaoLayoutATMHelper());