var EnumSituacaoAverbacaoFechamentoHelper = function () {
    this.Todas = 0;
    this.EmAberto = 1;
    this.EmFechamento = 2;
    this.Finalizada = 3;
};

EnumSituacaoAverbacaoFechamentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Em Aberto", value: this.EmAberto },
            { text: "Em Fechamento", value: this.EmFechamento },
            { text: "Finalizada", value: this.Finalizada }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoAverbacaoFechamento = Object.freeze(new EnumSituacaoAverbacaoFechamentoHelper());