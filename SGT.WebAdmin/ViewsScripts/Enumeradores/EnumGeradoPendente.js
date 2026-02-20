var EnumGeradoPendenteHelper = function () {
    this.Todos = 0;
    this.Pendente = 1;
    this.Gerado = 2;
};

EnumGeradoPendenteHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Pendente", value: this.Pendente },
            { text: "Gerado", value: this.Gerado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumGeradoPendente = Object.freeze(new EnumGeradoPendenteHelper());