var EnumSituacaoFaturamentoCTeHelper = function () {
    this.Todos = 0;
    this.Faturado = 1;
    this.NaoFaturado = 2;
};

EnumSituacaoFaturamentoCTeHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Faturado", value: this.Faturado },
            { text: "Não Faturado", value: this.NaoFaturado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoFaturamentoCTe = Object.freeze(new EnumSituacaoFaturamentoCTeHelper());