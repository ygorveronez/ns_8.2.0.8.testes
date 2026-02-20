var EnumSituacaoCheckinHelper = function () {
    this.Todas = "";
    this.SemConfirmacao = 0;
    this.Confirmado = 1;
    this.AguardandoAprovacao = 2;
    this.SemRegraAprovacao = 3;
    this.RecusaReprovada = 4;
    this.RecusaAprovada = 5;
};

EnumSituacaoCheckinHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aguardando Aprovação", value: this.AguardandoAprovacao },
            { text: "Confirmado", value: this.Confirmado },
            { text: "Recusa Aprovada", value: this.RecusaAprovada },
            { text: "Recusa Reprovada", value: this.RecusaReprovada },
            { text: "Sem Confirmação", value: this.SemConfirmacao },
            { text: "Sem Regra de Aprovação", value: this.SemRegraAprovacao }
        ];
    },
    obterOpcoesAprovacao: function () {
        return [
            { text: "Aguardando Aprovação", value: this.AguardandoAprovacao },
            { text: "Recusa Aprovada", value: this.RecusaAprovada },
            { text: "Recusa Reprovada", value: this.RecusaReprovada },
            { text: "Sem Regra de Aprovação", value: this.SemRegraAprovacao }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    },
    obterOpcoesPesquisaAprovacao: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoesAprovacao());
    }
};

var EnumSituacaoCheckin = Object.freeze(new EnumSituacaoCheckinHelper());
