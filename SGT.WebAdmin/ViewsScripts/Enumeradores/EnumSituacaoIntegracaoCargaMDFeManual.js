var EnumSituacaoIntegracaoCargaMDFeManualHelper = function () {
    this.Todas = "",
    this.AgIntegracao = 0;
    this.Integrado = 1;
    this.ProblemaIntegracao = 2;
    this.AgRetorno = 3;
};

EnumSituacaoIntegracaoCargaMDFeManualHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aguardando Integracões", value: this.AgIntegracao },
            { text: "Aguardando Retorno", value: this.AgRetorno },
            { text: "Integrado", value: this.Integrado },
            { text: "Falha ao Integrar", value: this.ProblemaIntegracao }
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

var EnumSituacaoIntegracaoCargaMDFeManual = Object.freeze(new EnumSituacaoIntegracaoCargaMDFeManualHelper());
