var EnumSituacaoContratoPrestacaoServicoHelper = function () {
    this.Todas = "";
    this.Aprovado = 1;
    this.AguardandoAprovacao = 2;
    this.SemRegraAprovacao = 3;
    this.AprovacaoRejeitada = 4;
};

EnumSituacaoContratoPrestacaoServicoHelper.prototype = {
    isPermiteAtualizar: function (situacao) {
        return (situacao != this.AguardandoAprovacao);
    },
    obterOpcoes: function () {
        return [
            { text: "Aguardando Aprovação", value: this.AguardandoAprovacao },
            { text: "Aprovação Rejeitada", value: this.AprovacaoRejeitada },
            { text: "Aprovado", value: this.Aprovado },
            { text: "Sem Regra de Aprovação", value: this.SemRegraAprovacao }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoContratoPrestacaoServico = Object.freeze(new EnumSituacaoContratoPrestacaoServicoHelper());