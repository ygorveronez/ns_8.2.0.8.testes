var EnumSituacaoFechamentoCargaHelper = function () {
    this.Todas = "";
    this.AgRateio = 0;
    this.AgCalculoFrete = 1;
    this.ProblemaCalculoFrete = 2;
    this.Finalizado = 3;
};

EnumSituacaoFechamentoCargaHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: "Ag. Rateio", value: this.AgAprovacaoSolicitacao },
            { text: "Ag. Calculo Frete", value: this.AgCancelamentoAverbacaoCTe },
            { text: "Problemas calculo Frete", value: this.ProblemaCalculoFrete },
            { text: "Finalizado", value: this.Finalizado }
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.ObterOpcoes());
    }
};

var EnumSituacaoFechamentoCarga = Object.freeze(new EnumSituacaoFechamentoCargaHelper());