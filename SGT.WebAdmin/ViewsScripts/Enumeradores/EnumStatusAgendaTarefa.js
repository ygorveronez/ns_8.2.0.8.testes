var EnumStatusAgendaTarefaHelper = function () {
    this.Todos = "";
    this.Aberto = 1;
    this.EmAndamento = 2;
    this.Cancelado = 3;
    this.Finalizado = 4;
};

EnumStatusAgendaTarefaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aberto", value: this.Aberto },
            { text: "Em Andamento", value: this.EmAndamento },
            { text: "Cancelado", value: this.Cancelado },
            { text: "Finalizado", value: this.Finalizado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumStatusAgendaTarefa = Object.freeze(new EnumStatusAgendaTarefaHelper());