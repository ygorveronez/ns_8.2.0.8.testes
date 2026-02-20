var EnumSituacaoGestaoDadosColetaHelper = function () {
    this.Todas = "";
    this.AguardandoAprovacao = 0;
    this.Aprovado = 1;
    this.Reprovado = 2;
};

EnumSituacaoGestaoDadosColetaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aguardando Aprovação", value: this.AguardandoAprovacao },
            { text: "Aprovado", value: this.Aprovado },
            { text: "Reprovado", value: this.Reprovado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
}

var EnumSituacaoGestaoDadosColeta = Object.freeze(new EnumSituacaoGestaoDadosColetaHelper());
