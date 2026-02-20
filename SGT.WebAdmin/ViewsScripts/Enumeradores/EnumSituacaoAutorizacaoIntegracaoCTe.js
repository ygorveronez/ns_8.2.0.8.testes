var EnumSituacaoAutorizacaoIntegracaoCTeHelper = function () {
    this.NaoInformado = 0;
    this.AguardandoAprovacao = 1;
    this.Aprovada = 2;
    this.Reprovada = 3;
}

EnumSituacaoAutorizacaoIntegracaoCTeHelper.prototype = {
    obterOpcoesPesquisaAutorizacao: function () {
        return [
            { text: "Todas", value: this.Todas },
            { text: "Aguardando Aprovação", value: this.AguardandoAprovacao },
            { text: "Aprovada", value: this.Aprovada },
            { text: "Reprovada", value: this.Reprovada }
        ];
    }
}

var EnumSituacaoAutorizacaoIntegracaoCTe = Object.freeze(new EnumSituacaoAutorizacaoIntegracaoCTeHelper());