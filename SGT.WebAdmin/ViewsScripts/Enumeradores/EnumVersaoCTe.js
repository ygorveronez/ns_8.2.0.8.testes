let EnumVersaoCTeHelper = function () {
    this.Versao300 = "3.00";
    this.Versao400 = "4.00";
};

EnumVersaoCTeHelper.prototype = {
    obterOpcoes: function () {
        return [
            {
                value: this.Versao300,
                text: "3.00"
            }, {
                value: this.Versao400,
                text: "4.00"
            }
        ];
    }
}

let EnumVersaoCTe = Object.freeze(new EnumVersaoCTeHelper());