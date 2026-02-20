var EnumPrioridadeGrupoPessoasHelper = function () {
    this.Outros = 0;
    this.TopPrioritario = 2;
};

EnumPrioridadeGrupoPessoasHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Outros", value: this.Outros },
            { text: "Top Prioritário", value: this.TopPrioritario },
        ];
    }
};

var EnumPrioridadeGrupoPessoas = Object.freeze(new EnumPrioridadeGrupoPessoasHelper());