var EnumStatusPedidoVendaHelper = function () {
    this.Todos = 0;
    this.Aberta = 1;
    this.Finalizada = 2;
    this.Faturada = 3;
    this.Cancelada = 4;
    this.AbertaFinalizada = 5;
    this.PendenteOperacional = 6;
    this.EmAprovacao = 7;
};

EnumStatusPedidoVendaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aberta", value: this.Aberta },
            { text: "Finalizada", value: this.Finalizada },
            { text: "Faturada", value: this.Faturada },
            { text: "Cancelada", value: this.Cancelada },
            { text: "Aberta/Finalizada", value: this.AbertaFinalizada },
            { text: "Pendente Operacional", value: this.PendenteOperacional },
            { text: "Em Aprovação", value: this.EmAprovacao }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumStatusPedidoVenda = Object.freeze(new EnumStatusPedidoVendaHelper());