var EnumIncontermHelper = function () {
    this.CIF = 1;
    this.FOBDirigido = 2;
    this.CIFFOBDirigido = 3;
    this.FOB = 4;
};

EnumIncontermHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "CIF", value: this.CIF },
            { text: "FOB Dirigido", value: this.FOBDirigido },
            { text: "CIF e FOB Dirigido", value: this.CIFFOBDirigido },
            { text: "FOB", value: this.FOB },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumInconterm = Object.freeze(new EnumIncontermHelper());