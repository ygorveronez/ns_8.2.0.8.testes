var EnumTipoRotaExtrattaHelper = function () {
    this.Todos = "";
    this.RotaDinamica = 0;
    this.RotaFixa = 1;
};

EnumTipoRotaExtrattaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Rota Dinâmica", value: this.RotaDinamica },
            { text: "Rota Fixa", value: this.RotaFixa }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoRotaExtratta = Object.freeze(new EnumTipoRotaExtrattaHelper());