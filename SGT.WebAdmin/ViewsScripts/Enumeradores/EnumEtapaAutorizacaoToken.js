var EnumEtapaAutorizacaoTokenHelper = function () {
    this.Todas = "";
    this.AprovacaoToken = 1;
};

EnumEtapaAutorizacaoTokenHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aprovação do Token", value: this.AprovacaoToken }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
}

var EnumEtapaAutorizacaoToken = Object.freeze(new EnumEtapaAutorizacaoTokenHelper());