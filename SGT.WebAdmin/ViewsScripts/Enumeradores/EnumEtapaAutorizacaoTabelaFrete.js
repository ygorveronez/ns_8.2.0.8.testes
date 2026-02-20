var EnumEtapaAutorizacaoTabelaFreteHelper = function () {
    this.Todas = 9;
    this.AprovacaoReajuste = 0;
    this.IntegracaoReajuste = 1;
};

EnumEtapaAutorizacaoTabelaFreteHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aprovação do Reajuste", value: this.AprovacaoReajuste },
            { text: "Integração do Reajuste", value: this.IntegracaoReajuste }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
}

var EnumEtapaAutorizacaoTabelaFrete = Object.freeze(new EnumEtapaAutorizacaoTabelaFreteHelper());