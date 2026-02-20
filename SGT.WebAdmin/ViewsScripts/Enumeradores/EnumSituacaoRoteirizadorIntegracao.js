var EnumSituacaoRoteirizadorIntegracaoHelper = function () {
    this.Todas = "";
    this.NaoIntegrado = 0;
    this.Integrado = 1;
    this.Cancelado = 2;
    this.FalhaIntegrar = 3;
};

EnumSituacaoRoteirizadorIntegracaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Cancelado", value: this.Cancelado },
            { text: "Integrado", value: this.Integrado },
            { text: "Não Integrado", value: this.NaoIntegrado },
            { text: "Falha Integrar", value: this.FalhaIntegrar }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoRoteirizadorIntegracao = Object.freeze(new EnumSituacaoRoteirizadorIntegracaoHelper);