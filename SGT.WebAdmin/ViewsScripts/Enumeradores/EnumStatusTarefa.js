let EnumStatusTarefaHelper = function () {
    this.Todos = "";
    this.Aguardando = 0;
    this.EmProcessamento = 1;
    this.Concluida = 2;
    this.Falha = 3;
};

EnumStatusTarefaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aguardando", value: this.Aguardando },
            { text: "Em Processamento", value: this.EmProcessamento },
            { text: "Concluída", value: this.Concluida },
            { text: "Falha", value: this.Falha },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.SituacaoPedido.Todos, value: this.Todos }].concat(this.obterOpcoes());
    },
};

let EnumStatusTarefa = Object.freeze(new EnumStatusTarefaHelper());