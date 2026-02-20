var EnumSituacaoProcessamentoFinalizacaoColetaEntregaEmLoteHelper = function () {
    this.Todos = null;
    this.PendenteFinalizacao = 0;
    this.EmFinalizacao = 1;
    this.Finalizado = 2;
    this.FalhaNaFinalizacao = 3;
};

EnumSituacaoProcessamentoFinalizacaoColetaEntregaEmLoteHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Todos", value: this.Todos },
            { text: "Pendente Finalização", value: this.PendenteFinalizacao },
            { text: "Em Finalização", value: this.EmFinalizacao },
            { text: "Finalizado", value: this.Finalizado },
            { text: "Falha na finalização", value: this.FalhaNaFinalizacao},
        ];
    },
}

var EnumSituacaoProcessamentoFinalizacaoColetaEntregaEmLote = Object.freeze(new EnumSituacaoProcessamentoFinalizacaoColetaEntregaEmLoteHelper());