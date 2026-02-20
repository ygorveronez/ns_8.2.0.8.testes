var EnumTipoFeriadoHelper = function () {
    this.Todos = "";
    this.Nacional = 0;
    this.Estadual = 1;
    this.Municipal = 2;
};

EnumTipoFeriadoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Estadual", value: this.Estadual },
            { text: "Municipal", value: this.Municipal },
            { text: "Nacional", value: this.Nacional }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumTipoFeriado = Object.freeze(new EnumTipoFeriadoHelper());
