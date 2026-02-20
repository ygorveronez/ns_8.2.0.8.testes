var EnumSituacaoLancamentoColaboradorHelper = function () {
    this.Todos = "";
    this.Agendado = 1;
    this.Cancelado = 2;
    this.Execucao = 3;
    this.Finalizado = 4;
};

EnumSituacaoLancamentoColaboradorHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Agendado", value: this.Agendado },
            { text: "Cancelado", value: this.Cancelado },
            { text: "Em Execução", value: this.Execucao },
            { text: "Finalizado", value: this.Finalizado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoLancamentoColaborador = Object.freeze(new EnumSituacaoLancamentoColaboradorHelper());