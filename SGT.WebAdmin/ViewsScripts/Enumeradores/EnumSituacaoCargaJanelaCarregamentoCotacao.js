var EnumSituacaoCargaJanelaCarregamentoCotacaoHelper = function () {
    this.Todas = "";
    this.NaoDefinida = 0;
    this.AguardandoAprovacao = 1;
    this.Aprovada = 2;
    this.Reprovada = 3;
    this.SemRegraAprovacao = 4;
};

EnumSituacaoCargaJanelaCarregamentoCotacaoHelper.prototype = {
    obterOpcoesAprovacao: function () {
        return [
            { text: "Aguardando Aprovação", value: this.AguardandoAprovacao },
            { text: "Aprovada", value: this.Aprovada },
            { text: "Reprovada", value: this.Reprovada },
            { text: "Sem Regra de Aprovação", value: this.SemRegraAprovacao }
        ];
    },
    obterOpcoesPesquisaAprovacao: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoesAprovacao());
    },
};

var EnumSituacaoCargaJanelaCarregamentoCotacao = Object.freeze(new EnumSituacaoCargaJanelaCarregamentoCotacaoHelper());
