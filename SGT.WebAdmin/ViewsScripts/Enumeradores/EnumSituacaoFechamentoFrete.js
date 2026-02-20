var EnumSituacaoFechamentoFreteHelper = function () {
    this.Todas = "";
    this.Aberto = 1;
    this.Fechado = 2;
    this.Cancelado = 3;
    this.EmEmissaoComplemento = 4;
    this.PendenciaEmissao = 5;
    this.AgIntegracao = 6;
    this.ProblemaIntegracao = 7;
}

EnumSituacaoFechamentoFreteHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Ag. Integração", value: this.AgIntegracao },
            { text: "Cancelado", value: this.Cancelado },
            { text: "Em Emissão Complementos", value: this.EmEmissaoComplemento },
            { text: "Finalizado", value: this.Fechado },
            { text: "Pendência na Emissão", value: this.PendenciaEmissao },
            { text: "Pendente", value: this.Aberto },
            { text: "Problema Integração", value: this.ProblemaIntegracao }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
}

var EnumSituacaoFechamentoFrete = Object.freeze(new EnumSituacaoFechamentoFreteHelper());
