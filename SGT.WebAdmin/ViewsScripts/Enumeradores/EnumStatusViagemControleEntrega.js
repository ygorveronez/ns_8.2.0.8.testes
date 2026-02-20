const EnumStatusViagemControleEntregaHelper = function () {
    this.Todas = "";
    this.Iniciada = 1;
    this.Finalizada = 2;
    this.NaoIniciada = 3;
    this.NaoFinalizada = 4;
    this.EmAndamento = 5;
};

EnumStatusViagemControleEntregaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.StatusViagemControleEntrega.EmAndamento, value: this.EmAndamento },
            { text: Localization.Resources.Enumeradores.StatusViagemControleEntrega.Iniciada, value: this.Iniciada },
            { text: Localization.Resources.Enumeradores.StatusViagemControleEntrega.Finalizada, value: this.Finalizada },
            { text: Localization.Resources.Enumeradores.StatusViagemControleEntrega.NaoIniciada, value: this.NaoIniciada },
            { text: Localization.Resources.Enumeradores.StatusViagemControleEntrega.NaoFinalizada, value: this.NaoFinalizada }
        ];
    },
    obterOpcoesAcompanhamentoCarga: function () {
        return [
            { text: Localization.Resources.Enumeradores.StatusViagemControleEntrega.EmAndamento, value: this.EmAndamento },
            { text: Localization.Resources.Enumeradores.StatusViagemControleEntrega.Finalizada, value: this.Finalizada },
            { text: Localization.Resources.Enumeradores.StatusViagemControleEntrega.NaoIniciada, value: this.NaoIniciada },
        ];
    },

    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.StatusViagemControleEntrega.Todas, value: this.Todas }].concat(this.obterOpcoes());
    },
    obterOpcoesAcompanhamentoCargaPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.StatusViagemControleEntrega.Todas, value: this.Todas }].concat(this.obterOpcoesAcompanhamentoCarga());
    }
}

const EnumStatusViagemControleEntrega = Object.freeze(new EnumStatusViagemControleEntregaHelper());