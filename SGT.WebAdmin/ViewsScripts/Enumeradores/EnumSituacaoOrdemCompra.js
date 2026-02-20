var EnumSituacaoOrdemCompraHelper = function () {
    this.Todas = "";
    this.AgAprovacao = 1;
    this.Aberta = 2;
    this.AgRetorno = 3;
    this.Finalizada = 4;
    this.Cancelada = 5;
    this.SemRegra = 6;
    this.Rejeitada = 7;
    this.Aprovada = 8;
    this.Incompleta = 9;
};

EnumSituacaoOrdemCompraHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Ag. Aprovação", value: this.AgAprovacao },
            { text: "Aberta", value: this.Aberta },
            { text: "Ag. Retorno", value: this.AgRetorno },
            { text: "Finalizada", value: this.Finalizada },
            { text: "Cancelada", value: this.Cancelada },
            { text: "Sem Regra", value: this.SemRegra },
            { text: "Rejeitada", value: this.Rejeitada },
            { text: "Aprovada", value: this.Aprovada },
            { text: "Incompleta", value: this.Incompleta }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoOrdemCompra = Object.freeze(new EnumSituacaoOrdemCompraHelper());