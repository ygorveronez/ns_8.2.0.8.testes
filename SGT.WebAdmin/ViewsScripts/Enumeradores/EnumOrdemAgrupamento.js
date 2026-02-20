var EnumOrdemAgrupamentoHelper = function () {
    this.Crescente = "asc";
    this.Decrescente = "desc";
};

EnumOrdemAgrupamentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.OrdemAgrupamento.Crescente, value: this.Crescente },
            { text: Localization.Resources.Enumeradores.OrdemAgrupamento.Descrescente, value: this.Decrescente },
        ];
    },
}

var EnumOrdemAgrupamento = Object.freeze(new EnumOrdemAgrupamentoHelper());