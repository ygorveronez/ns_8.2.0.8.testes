var EnumSituacaoIntegracaoCargaEDIHelper = function () {
    this.Todos = "";
    this.AgIntegracao = 1;
    this.Integrado = 2;
    this.Falha = 3;
};

EnumSituacaoIntegracaoCargaEDIHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacaoIntegracaoCargaEDI.Todos, value: this.Todos },
            { text: Localization.Resources.Enumeradores.SituacaoIntegracaoCargaEDI.AguardandoIntegracao, value: this.AgIntegracao },
            { text: Localization.Resources.Enumeradores.SituacaoIntegracaoCargaEDI.Falha, value: this.Falha },
            { text: Localization.Resources.Enumeradores.SituacaoIntegracaoCargaEDI.Integrado, value: this.Integrado }
        ];
    }
}

var EnumSituacaoIntegracaoCargaEDI = Object.freeze(new EnumSituacaoIntegracaoCargaEDIHelper());