var EnumResponsavelSeguroHelper = function () {
    this.Todos = 9;
    this.Transportador = 1;
    this.Embarcador = 2;
};

EnumResponsavelSeguroHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Embarcador", value: this.Embarcador },
            { text: "Transportador", value: this.Transportador }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumResponsavelSeguro = Object.freeze(new EnumResponsavelSeguroHelper());