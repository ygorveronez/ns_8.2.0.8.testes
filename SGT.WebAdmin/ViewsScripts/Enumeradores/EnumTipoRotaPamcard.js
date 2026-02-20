var EnumTipoRotaPamcardHelper = function () {
    this.Todos = "";
    this.RotaFixa = 0;
    this.RotaDinamica = 1;
};

EnumTipoRotaPamcardHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Rota Fixa", value: this.RotaFixa },
            { text: "Rota Dinamica", value: this.RotaDinamica }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoRotaPamcard = Object.freeze(new EnumTipoRotaPamcardHelper());