var EnumSituacaoAgendamentoPalletHelper = function () {
    this.Todas = "";
    this.Agendamento = 1;
    this.Acompanhamento = 2;
    this.Finalizado = 3;
    this.Cancelado = 4;
};

EnumSituacaoAgendamentoPalletHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Agendamento", value: this.Agendamento },
            { text: "Acompanhamento", value: this.Acompanhamento },
            { text: "Concluído", value: this.Finalizado },
            { text: "Cancelado", value: this.Cancelado }
        ];
    },

    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.SituacaoAgendamento.Todas, value: this.Todas }].concat(this.obterOpcoes());
    },

};

var EnumSituacaoAgendamentoPallet = Object.freeze(new EnumSituacaoAgendamentoPalletHelper());