var EnumSituacaoIntegracaoTabelaFreteClienteHelper = function () {
    this.Todas = "";
    this.AguardandoIntegracao = 1;
    this.FalhaIntegracao = 2;
    this.Integrado = 3;
    this.AguardandoRetorno = 4;
};

EnumSituacaoIntegracaoTabelaFreteClienteHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacaoIntegracaoTabelaFreteCliente.AguardandoIntegracao, value: this.AguardandoIntegracao },
            { text: Localization.Resources.Enumeradores.SituacaoIntegracaoTabelaFreteCliente.AguardandoRetorno, value: this.AguardandoRetorno },
            { text: Localization.Resources.Enumeradores.SituacaoIntegracaoTabelaFreteCliente.FalhaIntegracao, value: this.FalhaIntegracao },
            { text: Localization.Resources.Enumeradores.SituacaoIntegracaoTabelaFreteCliente.Integrado, value: this.Integrado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todas, value: this.Todas }].concat(this.obterOpcoes());
    }
}

var EnumSituacaoIntegracaoTabelaFreteCliente = Object.freeze(new EnumSituacaoIntegracaoTabelaFreteClienteHelper());
