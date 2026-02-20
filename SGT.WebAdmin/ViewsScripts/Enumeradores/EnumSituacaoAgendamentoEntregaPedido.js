var EnumSituacaoAgendamentoEntregaPedidoHelper = function () {
    this.Todas = "";
    this.AguardandoAgendamento = 1;
    this.Agendado = 2;
    this.Finalizado = 3;
    this.ReagendamentoSolicitado = 4;
    this.AguardandoRetornoCliente = 5;
    this.AguardandoReagendamento = 6;
    this.Reagendado = 7;
    this.NaoExigeAgendamento = 8;
};

EnumSituacaoAgendamentoEntregaPedidoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aguardando Agendamento", value: this.AguardandoAgendamento },
            { text: "Agendado", value: this.Agendado },
            { text: "Finalizado", value: this.Finalizado },
            { text: "Reagendamento Solicitado", value: this.ReagendamentoSolicitado },
            { text: "Aguardando Retorno Cliente", value: this.AguardandoRetornoCliente },
            { text: "Aguardando Reagendamento", value: this.AguardandoReagendamento },
            { text: "Reagendado", value: this.Reagendado },
            { text: "Não exige agendamento", value: this.NaoExigeAgendamento }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
}

var EnumSituacaoAgendamentoEntregaPedido = Object.freeze(new EnumSituacaoAgendamentoEntregaPedidoHelper());
