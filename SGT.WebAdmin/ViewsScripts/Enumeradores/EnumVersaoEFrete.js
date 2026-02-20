let EnumVersaoEFreteHelper = function () {
    this.Versao1  = 0;
    this.Versao2  = 1;
};

EnumVersaoEFreteHelper.prototype = {
    obterOpcoes: function () {
        return [
            {
                value: this.Versao1,
                text: "Versão 1"
            }, {
                value: this.Versao2,
                text: "Versão 2"
            }
        ];
    }
}

let EnumVersaoEFrete = Object.freeze(new EnumVersaoEFreteHelper());