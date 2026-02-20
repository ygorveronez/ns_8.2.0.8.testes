var EnumSituacaoCargaCancelamentoSolicitacaoHelper = function () {
    this.Todas = "";
    this.AguardandoAprovacao = 1;
    this.Aprovada = 2;
    this.Reprovada = 3;
    this.SemRegraAprovacao = 4;
};

EnumSituacaoCargaCancelamentoSolicitacaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aguardando Aprovação", value: this.AguardandoAprovacao },
            { text: "Aprovada", value: this.Aprovada },
            { text: "Reprovada", value: this.Reprovada },
            { text: "Sem Regra Aprovação", value: this.SemRegraAprovacao },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoCargaCancelamentoSolicitacao = Object.freeze(new EnumSituacaoCargaCancelamentoSolicitacaoHelper());
