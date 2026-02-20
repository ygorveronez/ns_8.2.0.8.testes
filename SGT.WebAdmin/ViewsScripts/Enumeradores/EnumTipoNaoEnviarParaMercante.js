var EnumTipoNaoEnviarParaMercanteHelper = function () {
    this.Todos = 0;
    this.InclusaoNFe = 1;
    this.AnulacaoValoresRecusa = 2;
    this.Outros = 9;
};

EnumTipoNaoEnviarParaMercanteHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Inclusão de NF-e", value: this.InclusaoNFe },
            { text: "Anulação de valores / Recusa", value: this.AnulacaoValoresRecusa },
            { text: "Outros", value: this.Outros }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoNaoEnviarParaMercante = Object.freeze(new EnumTipoNaoEnviarParaMercanteHelper());