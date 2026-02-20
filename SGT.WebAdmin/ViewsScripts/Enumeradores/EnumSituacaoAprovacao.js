var EnumSituacaoAprovacaoHelper = function () {
    this.Todas = "";
    this.AguardandoAprovacao = 1;
    this.Aprovada = 2;
    this.Reprovada = 3;
    this.SemRegraAprovacao = 4;
}

EnumSituacaoAprovacaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aguardando Aprovação", value: this.AguardandoAprovacao },
            { text: "Aprovado", value: this.Aprovada },
            { text: "Reprovado", value: this.Reprovada },
            { text: "Sem Regra de Aprovação", value: this.SemRegraAprovacao }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
}

var EnumSituacaoAprovacao = Object.freeze(new EnumSituacaoAprovacaoHelper());