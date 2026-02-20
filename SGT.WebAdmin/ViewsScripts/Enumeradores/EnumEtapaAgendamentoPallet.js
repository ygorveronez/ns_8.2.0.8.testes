let EnumEtapaAgendamentoPalletHelper = function () {
    this.Todos = "";
    this.Agendamento = 0;
    this.NFe = 1;
    this.Acompanhamento = 2;
};

EnumEtapaAgendamentoPalletHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Agendamento", value: this.Agendamento },
            { text: "NFe", value: this.NFe },
            { text: "Acompanhamento", value: this.Acompanhamento },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.SituacaoPedido.Todos, value: this.Todos }].concat(this.obterOpcoes());
    },
};

let EnumEtapaAgendamentoPallet = Object.freeze(new EnumEtapaAgendamentoPalletHelper());