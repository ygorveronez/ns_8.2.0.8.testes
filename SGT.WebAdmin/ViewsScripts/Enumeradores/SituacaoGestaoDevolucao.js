var EnumSituacaoGestaoDevolucaoHelper = function () {
    this.Ativa = 0;
    this.AnaliseCancelamento = 1;
    this.Cancelada = 2;
    this.Finalizada = 3;
};

EnumSituacaoGestaoDevolucaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Ativa", value: this.Ativa },
            { text: "Análise de Cancelamento", value: this.AnaliseCancelamento },
            { text: "Cancelada", value: this.Cancelada },
            { text: "Finalizada", value: this.Finalizada }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [
            { text: "Ativa", value: this.Ativa },
            { text: "Análise de Cancelamento", value: this.AnaliseCancelamento },
            { text: "Cancelada", value: this.Cancelada },
            { text: "Finalizada", value: this.Finalizada }
        ];
    }
};

var EnumSituacaoGestaoDevolucao = Object.freeze(new EnumSituacaoGestaoDevolucaoHelper());