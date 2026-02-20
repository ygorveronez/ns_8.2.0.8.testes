var EnumVersaoIntegracaoTrizyHelper = function() {
    this.Versao1 = 1,
    this.Versao3 = 3
}

EnumVersaoIntegracaoTrizyHelper.prototype = {
    obterOpcoes: function () {
        return [
            {
                text: "Versão 1",
                value: this.Versao1
            }, {
                text: "Versão 3",
                value: this.Versao3
            }
        ];
    }
}

let EnumVersaoIntegracaoTrizy = Object.freeze(new EnumVersaoIntegracaoTrizyHelper());