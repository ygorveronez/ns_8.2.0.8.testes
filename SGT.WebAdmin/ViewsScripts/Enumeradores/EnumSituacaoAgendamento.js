var EnumSitaucaoAgendamentoHelper = function () {
    this.Todas = 0;
    this.Confirmado = 1;
    this.PendenteAgendamento = 2;
};

EnumSitaucaoAgendamentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacaoAgendamento.Confirmado, value: this.Confirmado },
            { text: Localization.Resources.Enumeradores.SituacaoAgendamento.PendenteAgendamento, value: this.PendenteAgendamento }
        ];
    },

    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.SituacaoAgendamento.Todas, value: this.Todas }].concat(this.obterOpcoes());
    },

};

var EnumSituacaoAgendamento = Object.freeze(new EnumSitaucaoAgendamentoHelper());