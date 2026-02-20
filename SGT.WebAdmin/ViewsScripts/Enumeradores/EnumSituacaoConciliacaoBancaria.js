var EnumSituacaoConciliacaoBancariaHelper = function () {
    this.Todas = 0;
    this.Aberto = 1;
    this.Finalizado = 2;
    this.Cancelado = 3;
};

EnumSituacaoConciliacaoBancariaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aberto", value: this.Aberto },
            { text: "Finalizado", value: this.Finalizado },
            { text: "Cancelado", value: this.Cancelado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoConciliacaoBancaria = Object.freeze(new EnumSituacaoConciliacaoBancariaHelper());