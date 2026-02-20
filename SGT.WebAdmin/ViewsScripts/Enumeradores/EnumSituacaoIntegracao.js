const EnumSituacaoIntegracaoHelper = function () {
    this.Todas = null;
    this.AgIntegracao = 0;
    this.Integrado = 1;
    this.ProblemaIntegracao = 2;
    this.AgRetorno = 3;
};

EnumSituacaoIntegracaoHelper.prototype = {
    obterOpcoes: function (exibirOpcaoCancelada) {
        const opcoes = [];

        opcoes.push({ text: Localization.Resources.Enumeradores.SituacaoIntegracao.AguardandoIntegracao, value: this.AgIntegracao });
        opcoes.push({ text: Localization.Resources.Enumeradores.SituacaoIntegracao.AguardandoRetorno, value: this.AgRetorno });

        if (exibirOpcaoCancelada)
            opcoes.push({ text: Localization.Resources.Enumeradores.SituacaoIntegracao.Cancelada, value: 99 });

        opcoes.push({ text: Localization.Resources.Enumeradores.SituacaoIntegracao.Integrado, value: this.Integrado });
        opcoes.push({ text: Localization.Resources.Enumeradores.SituacaoIntegracao.FalhaAoIntegrar, value: this.ProblemaIntegracao });

        return opcoes;
    },
    obterOpcoesPesquisa: function (exibirOpcaoCancelada) {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todas }].concat(this.obterOpcoes(exibirOpcaoCancelada));
    }
};

const EnumSituacaoIntegracao = Object.freeze(new EnumSituacaoIntegracaoHelper());
