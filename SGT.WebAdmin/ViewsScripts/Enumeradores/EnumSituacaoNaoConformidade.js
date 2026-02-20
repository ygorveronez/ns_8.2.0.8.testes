var EnumSituacaoNaoConformidadeHelper = function () {
    this.Todas = "";
    this.AguardandoTratativa = 1;
    this.Concluida = 2;
    this.Reprovada = 3;
    this.SemRegraAprovacao = 4;
    this.ConcluidaPorIntegracao = 5;
    this.AprovadaEmContingencia = 6;
};

EnumSituacaoNaoConformidadeHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aguardando tratativa", value: this.AguardandoTratativa },
            { text: "Aprovada em contingência", value: this.AprovadaEmContingencia },
            { text: "Concluída por integração", value: this.ConcluidaPorIntegracao },
            { text: "Concluída", value: this.Concluida },
            { text: "Reprovada", value: this.Reprovada },
            { text: "Sem Regra de Aprovação", value: this.SemRegraAprovacao }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    },
};

var EnumSituacaoNaoConformidade = Object.freeze(new EnumSituacaoNaoConformidadeHelper());
