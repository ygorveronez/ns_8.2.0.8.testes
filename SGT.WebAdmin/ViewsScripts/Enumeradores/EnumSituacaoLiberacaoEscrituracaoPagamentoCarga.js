var EnumSituacaoLiberacaoEscrituracaoPagamentoCargaHelper = function () {
    this.Todas = "";
    this.NaoInformada = 0;
    this.AguardandoAprovacao = 1;
    this.Aprovada = 2;
    this.Reprovada = 3;
    this.SemRegraAprovacao = 4;
}

EnumSituacaoLiberacaoEscrituracaoPagamentoCargaHelper.prototype = {
    obterOpcoesPesquisaAutorizacao: function () {
        return [
            { text: "Todas", value: this.Todas },
            { text: "Aguardando Aprovação", value: this.AguardandoAprovacao },
            { text: "Aprovada", value: this.Aprovada },
            { text: "Reprovada", value: this.Reprovada },
            { text: "Sem Regra de Aprovação", value: this.SemRegraAprovacao },
        ];
    }
};

var EnumSituacaoLiberacaoEscrituracaoPagamentoCarga = Object.freeze(new EnumSituacaoLiberacaoEscrituracaoPagamentoCargaHelper());