var EnumSituacaoGestaoDadosColetaRetornoConfirmacaoHelper = function () {
    this.Todas = "";
    this.SemErro = 0;
    this.ComErro = 1;
};

EnumSituacaoGestaoDadosColetaRetornoConfirmacaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Sem erro", value: this.SemErro },
            { text: "Com erro", value: this.ComErro }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
}

var EnumSituacaoGestaoDadosColetaRetornoConfirmacao = Object.freeze(new EnumSituacaoGestaoDadosColetaRetornoConfirmacaoHelper());
