var EnumTipoMovimentacaoEstoquePalletHelper = function () {
    this.Todas = "";
    this.Entrada = 0;
    this.Saida = 1;
};

EnumTipoMovimentacaoEstoquePalletHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { value: this.Entrada, text: "Entrada" },
            { value: this.Saida, text: "Saída" }
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ value: this.Todas, text: "Todas" }].concat(this.ObterOpcoes());
    }
};

var EnumTipoMovimentacaoEstoquePallet = Object.freeze(new EnumTipoMovimentacaoEstoquePalletHelper());