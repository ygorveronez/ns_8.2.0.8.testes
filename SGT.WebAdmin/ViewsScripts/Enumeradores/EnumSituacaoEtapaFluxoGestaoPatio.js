var EnumSituacaoEtapaFluxoGestaoPatioHelper = function () {
    this.Todas = "";
    this.Aguardando = 1;
    this.Aprovado = 2;
    this.Rejeitado = 3;
    this.Cancelado = 4;
};

EnumSituacaoEtapaFluxoGestaoPatioHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacaoEtapaFluxoGestaoPatio.Aguardando, value: this.Aguardando },
            { text: Localization.Resources.Enumeradores.SituacaoEtapaFluxoGestaoPatio.Aprovado, value: this.Aprovado },
            { text: Localization.Resources.Enumeradores.SituacaoEtapaFluxoGestaoPatio.Rejeitado, value: this.Rejeitado },
            { text: Localization.Resources.Enumeradores.SituacaoEtapaFluxoGestaoPatio.Cancelado, value: this.Cancelado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{
            text: Localization.Resources.Enumeradores.SituacaoEtapaFluxoGestaoPatio.Todas, value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoEtapaFluxoGestaoPatio = Object.freeze(new EnumSituacaoEtapaFluxoGestaoPatioHelper());