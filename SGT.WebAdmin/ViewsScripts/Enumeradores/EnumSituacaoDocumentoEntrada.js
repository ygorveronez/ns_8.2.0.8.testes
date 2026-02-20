var EnumSituacaoDocumentoEntradaHelper = function () {
    this.Todos = "";
    this.Aberto = 1;
    this.Cancelado = 2;
    this.Finalizado = 3;
    this.Anulado = 4;
};

EnumSituacaoDocumentoEntradaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aberto", value: this.Aberto },
            { text: "Cancelado", value: this.Cancelado },
            { text: "Finalizado", value: this.Finalizado },
            { text: "Anulado", value: this.Anulado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoDocumentoEntrada = Object.freeze(new EnumSituacaoDocumentoEntradaHelper());