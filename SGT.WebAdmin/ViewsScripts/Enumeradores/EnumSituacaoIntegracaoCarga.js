var EnumSituacaoIntegracaoCargaHelper = function () {
    this.Todas = "",
    this.AgIntegracao = 0;
    this.Integrado = 1;
    this.ProblemaIntegracao = 2;
    this.AgRetorno = 3;
};

EnumSituacaoIntegracaoCargaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacaoIntegracaoCarga.AguardandoIntegracao, value: this.AgIntegracao },
            { text: Localization.Resources.Enumeradores.SituacaoIntegracaoCarga.AguardandoRetorno, value: this.AgRetorno },
            { text: Localization.Resources.Enumeradores.SituacaoIntegracaoCarga.Integrado, value: this.Integrado },
            { text: Localization.Resources.Enumeradores.SituacaoIntegracaoCarga.FalhaAoIntegrar, value: this.ProblemaIntegracao }
        ];
    },
    obterOpcoesSemAguardandoRetorno: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacaoIntegracaoCarga.AguardandoIntegracao, value: this.AgIntegracao },
            { text: Localization.Resources.Enumeradores.SituacaoIntegracaoCarga.Integrado, value: this.Integrado },
            { text: Localization.Resources.Enumeradores.SituacaoIntegracaoCarga.FalhaAoIntegrar, value: this.ProblemaIntegracao }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todas }].concat(this.obterOpcoes());
    },
    obterOpcoesPesquisaSemAguardandoRetorno: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todas }].concat(this.obterOpcoesSemAguardandoRetorno());
    }
};

var EnumSituacaoIntegracaoCarga = Object.freeze(new EnumSituacaoIntegracaoCargaHelper());
