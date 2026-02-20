var EnumSituacaoAgendamentoEntregaHelper = function () {
    this.Todas = "";
    this.AguardandoConfirmacao = 1;
    this.Agendado = 2;
    this.Finalizado = 3;
};

EnumSituacaoAgendamentoEntregaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aguardando Confirmação", value: this.AguardandoConfirmacao },
            { text: "Agendado", value: this.Agendado },
            { text: "Finalizado", value: this.Finalizado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
}

var EnumSituacaoAgendamentoEntrega = Object.freeze(new EnumSituacaoAgendamentoEntregaHelper());
