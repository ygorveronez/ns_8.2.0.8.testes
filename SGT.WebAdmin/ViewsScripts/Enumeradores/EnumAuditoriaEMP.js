var EnumSituacaoIntegracaoEMPHelper = function () {
    this.NotPersist = 0;
    this.Integrado = 1;
    this.ErroResolvido = 2;
    this.NaoInformado = 10;
};

EnumSituacaoIntegracaoEMPHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Todos", value: this.NaoInformado },
            { text: "Not Persist", value: this.NotPersist },
            { text: "Integrado", value: this.Integrado },
            { text: "Erro Resolvido", value: this.ErroResolvido }
        ];
    },
};

var EnumSituacaoIntegracaoEMP = Object.freeze(new EnumSituacaoIntegracaoEMPHelper());