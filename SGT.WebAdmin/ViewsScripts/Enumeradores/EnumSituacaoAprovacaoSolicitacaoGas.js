var EnumSituacaoAprovacaoSolicitacaoGasHelper = function () {
    this.Todas = "";
    this.AguardandoAprovacao = 1;
    this.Aprovada = 2;
    this.Reprovada = 3;
    this.SemRegraAprovacao = 4;
}

EnumSituacaoAprovacaoSolicitacaoGasHelper.prototype = {
    obterOpcoesSolicitacaoGas: function () {
        return [
            { text: "Valores Aguardando Aprovação", value: this.AguardandoAprovacao },
            { text: "Valores Aprovados", value: this.Aprovada },
            { text: "Valores Reprovados", value: this.Reprovada },
            { text: "Sem Regra de Aprovação", value: this.SemRegraAprovacao }
        ];
    },
    obterOpcoesPesquisaSolicitacaoGas: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoesSolicitacaoGas());
    }
}

var EnumSituacaoAprovacaoSolicitacaoGas = Object.freeze(new EnumSituacaoAprovacaoSolicitacaoGasHelper());