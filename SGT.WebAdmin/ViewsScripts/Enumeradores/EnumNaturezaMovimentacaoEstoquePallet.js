var EnumNaturezaMovimentacaoEstoquePalletHelper = function () {
    this.Todas = "";
    this.Avaria = 1;
    this.Cliente = 2;
    this.Filial = 3;
    this.Reforma = 4;
    this.Transportador = 5;
}

EnumNaturezaMovimentacaoEstoquePalletHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Todas", value: this.Todas },
            { text: "Avaria", value: this.Avaria },
            { text: "Cliente", value: this.Cliente },
            { text: "Filial", value: this.Filial },
            { text: "Reforma", value: this.Reforma },
            { text: "Transportador", value: this.Transportador }
        ];
    }
}

var EnumNaturezaMovimentacaoEstoquePallet = Object.freeze(new EnumNaturezaMovimentacaoEstoquePalletHelper());