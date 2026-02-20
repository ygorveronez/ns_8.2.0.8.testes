var EnumSituacaoFechamentoPontuacaoHelper = function () {
    this.Todas = "";
    this.AguardandoFinalizacao = 1;
    this.Cancelado = 2;
    this.Finalizado = 3;
};

EnumSituacaoFechamentoPontuacaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aguardando Finalização", value: this.AguardandoFinalizacao },
            { text: "Cancelado", value: this.Cancelado },
            { text: "Finalizado", value: this.Finalizado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoFechamentoPontuacao = Object.freeze(new EnumSituacaoFechamentoPontuacaoHelper());