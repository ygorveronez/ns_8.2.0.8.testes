let EnumSituacaoPesagemCargaHelper = function () {
    this.Todas = "";
    this.NaoInformada = 0;
    this.AguardandoAprovacao = 1;
    this.Aprovada = 2;
    this.Reprovada = 3;
    this.SemRegraAprovacao = 4;
};

EnumSituacaoPesagemCargaHelper.prototype = {
    isPermiteAvancarEtapa: function (situacao) {
        return (situacao == this.Aprovada) || (situacao == this.NaoInformada) || (situacao == this.Reprovada);
    },
    obterOpcoesPesquisaAutorizacao: function () {
        return [
            { text: "Todas", value: this.Todas },
            { text: "Aguardando Aprovação", value: this.AguardandoAprovacao },
            { text: "Aprovada", value: this.Aprovada },
            { text: "Reprovada", value: this.Reprovada },
            { text: "Sem Regra Aprovação", value: this.SemRegraAprovacao },
        ];
    }
};

let EnumSituacaoPesagemCarga = Object.freeze(new EnumSituacaoPesagemCargaHelper());