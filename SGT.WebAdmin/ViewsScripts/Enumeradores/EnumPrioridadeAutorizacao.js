var EnumPrioridadeAutorizacaoHelper = function () {
    this.Todas = "";
    this.Zero = 0;
    this.Um = 1;
    this.Dois = 2;
    this.Tres = 3;
    this.Quatro = 4;
    this.Cinco = 5;
};

EnumPrioridadeAutorizacaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "0", value: this.Zero },
            { text: "1", value: this.Um },
            { text: "2", value: this.Dois },
            { text: "3", value: this.Tres },
            { text: "4", value: this.Quatro },
            { text: "5", value: this.Cinco }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumPrioridadeAutorizacao = Object.freeze(new EnumPrioridadeAutorizacaoHelper());