const EnumTipoFreteHelper = function () {
    this.CIFFOB = '';
    this.CIF = 1;
    this.FOB = 2;
};

EnumTipoFreteHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: "CIF e FOB", value: this.CIFFOB },
            { text: "CIF", value: this.CIF },
            { text: "FOB", value: this.FOB },
        ];
    }
}

const EnumTipoFrete = Object.freeze(new EnumTipoFreteHelper());