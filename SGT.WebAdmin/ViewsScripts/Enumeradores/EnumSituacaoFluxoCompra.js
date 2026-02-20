var EnumSituacaoFluxoCompraHelper = function () {
    this.Todos = 0;
    this.Aberto = 1;
    this.Finalizado = 2;
    this.Cancelado = 3;
    this.Rejeitado = 4;
};

EnumSituacaoFluxoCompraHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aberto", value: this.Aberto },
            { text: "Finalizado", value: this.Finalizado },
            { text: "Cancelado", value: this.Cancelado },
            { text: "Rejeitado", value: this.Rejeitado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoFluxoCompra = Object.freeze(new EnumSituacaoFluxoCompraHelper());