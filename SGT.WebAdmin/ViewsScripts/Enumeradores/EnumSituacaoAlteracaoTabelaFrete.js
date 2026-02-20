var EnumSituacaoAlteracaoTabelaFreteHelper = function () {
    this.Todas = "";
    this.AguardandoAprovacao = 1;
    this.Aprovada = 2;
    this.Reprovada = 3;
    this.SemRegraAprovacao = 4;
}

EnumSituacaoAlteracaoTabelaFreteHelper.prototype = {
    obterOpcoesTabelaFrete: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacaoAlteracaoTabelaFrete.AguardandoAprovacao, value: this.AguardandoAprovacao },
            { text: Localization.Resources.Enumeradores.SituacaoAlteracaoTabelaFrete.Aprovada, value: this.Aprovada },
            { text: Localization.Resources.Enumeradores.SituacaoAlteracaoTabelaFrete.Reprovada, value: this.Reprovada },
            { text: Localization.Resources.Enumeradores.SituacaoAlteracaoTabelaFrete.SemRegraDeAprovacao, value: this.SemRegraAprovacao }
        ];
    },
    obterOpcoesTabelaFreteCliente: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacaoAlteracaoTabelaFrete.AguardandoAprovacao, value: this.AguardandoAprovacao },
            { text: Localization.Resources.Enumeradores.SituacaoAlteracaoTabelaFrete.Aprovada, value: this.Aprovada },
            { text: Localization.Resources.Enumeradores.SituacaoAlteracaoTabelaFrete.Reprovada, value: this.Reprovada },
            { text: Localization.Resources.Enumeradores.SituacaoAlteracaoTabelaFrete.SemRegraDeAprovacao, value: this.SemRegraAprovacao }
        ];
    },
    obterOpcoesPesquisaTabelaFrete: function () {
        return [{ text: Localization.Resources.Enumeradores.SituacaoAlteracaoTabelaFrete.Todas, value: this.Todas }].concat(this.obterOpcoesTabelaFrete());
    },
    obterOpcoesPesquisaTabelaFreteCliente: function () {
        return [{ text: Localization.Resources.Enumeradores.SituacaoAlteracaoTabelaFrete.Todas, value: this.Todas }].concat(this.obterOpcoesTabelaFreteCliente());
    }
}

var EnumSituacaoAlteracaoTabelaFrete = Object.freeze(new EnumSituacaoAlteracaoTabelaFreteHelper());