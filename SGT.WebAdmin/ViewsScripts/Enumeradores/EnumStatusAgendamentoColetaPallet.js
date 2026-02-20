let EnumStatusAgendamentoColetaPalletHelper = function () {
    this.Todos = "";
    this.EmAndamento = 0;
    this.Finalizado = 1;
    this.Cancelado = 2;
};

EnumStatusAgendamentoColetaPalletHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Em Andamento", value: this.EmAndamento },
            { text: "Finalizado", value: this.Finalizado },
            { text: "Cancelado", value: this.Cancelado },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.SituacaoPedido.Todos, value: this.Todos }].concat(this.obterOpcoes());
    },
};

let EnumStatusAgendamentoColetaPallet = Object.freeze(new EnumStatusAgendamentoColetaPalletHelper());