var EnumTipoComponenteFreteHelper = function () {
    this.TODOS = 0;
    this.ICMS = 1;
    this.PEDAGIO = 2;
    this.DESCARGA = 3;
    this.FRETE = 4;
    this.ADVALOREM = 5;
    this.ISS = 6;
    this.PISCONFIS = 13;
    this.OUTROS = 9;
    this.GRIS = 14;
    this.ENTREGA = 15;
    this.PERNOITE = 16;
}

EnumTipoComponenteFreteHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Outros", value: this.OUTROS },
            { text: "Valor de Frete", value: this.FRETE },
            { text: "Pedagio", value: this.PEDAGIO },
            { text: "Descarga", value: this.DESCARGA },
            { text: "AD VALOREM", value: this.ADVALOREM },
            { text: "ICMS", value: this.ICMS },
            { text: "ISS", value: this.ISS },
            { text: "GRIS", value: this.GRIS },
            { text: "PIS e COFINS", value: this.PISCONFIS },
            { text: "Entrega", value: this.ENTREGA },
            { text: "Pernoite", value: this.PERNOITE }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.TODOS }].concat(this.obterOpcoes());
    }
};

var EnumTipoComponenteFrete = Object.freeze(new EnumTipoComponenteFreteHelper());
