var EnumSituacaoSessaoRoteirizadorHelper = function () {
    this.Todas = 0;
    this.Iniciada = 1;
    this.Finalizada = 2;
    this.Cancelada = 3;
};

EnumSituacaoSessaoRoteirizadorHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacaoSessaoRoteirizador.UmIniciada, value: this.Iniciada },
            { text: Localization.Resources.Enumeradores.SituacaoSessaoRoteirizador.DoisFinalizada, value: this.Finalizada },
            { text: Localization.Resources.Enumeradores.SituacaoSessaoRoteirizador.TresCancelada, value: this.Cancelada }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.SituacaoSessaoRoteirizador.ZeroTodas, value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoSessaoRoteirizador = Object.freeze(new EnumSituacaoSessaoRoteirizadorHelper());