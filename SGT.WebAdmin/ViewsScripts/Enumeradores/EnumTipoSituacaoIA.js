var EnumTipoSituacaoIA = function () {
    this.Todas = 0;
    this.AguardandoIntegracao = 1;
    this.AguardandoRetorno = 2;
    this.Integrado = 3;
    this.FalhaIntegracao = 4;
};

EnumTipoSituacaoIA.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aguardando Integração", value: this.AguardandoIntegracao },
            { text: "Aguardando Retorno", value: this.AguardandoRetorno },
            { text: "Integrado", value: this.Integrado },
            { text: "Falha Integração", value: this.FalhaIntegracao }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    },
};

var EnumTipoSituacaoIA = Object.freeze(new EnumTipoSituacaoIA());