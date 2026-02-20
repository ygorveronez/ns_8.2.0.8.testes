var EnumOrigemDigitalizacaoCanhotoHelper = function () {
    this.Todas = 0;
    this.Mobile = 1;
    this.Portal = 2;
    this.MobileSemValidacaoAut = 4;
};

EnumOrigemDigitalizacaoCanhotoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Mobile", value: this.Mobile },
            { text: "Portal", value: this.Portal },
            { text: "Mobile sem validação aut.", value: this.MobileSemValidacaoAut },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumOrigemDigitalizacaoCanhoto = Object.freeze(new EnumOrigemDigitalizacaoCanhotoHelper());