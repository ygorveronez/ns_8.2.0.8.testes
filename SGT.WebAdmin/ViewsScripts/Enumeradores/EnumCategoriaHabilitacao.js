var EnumEnumCategoriaHabilitacaoHelper = function () {
    this.Nenhuma = "";
    this.CategoriaA = 1;
    this.CategoriaB = 2;
    this.CategoriaC = 3;
    this.CategoriaD = 4;
    this.CategoriaE = 5;
    this.CategoriaAB = 6;
    this.CategoriaAC = 7;
    this.CategoriaAD = 8;
    this.CategoriaAE = 9;
};

EnumEnumCategoriaHabilitacaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Nenhuma", value: this.Nenhuma },
            { text: "A", value: this.CategoriaA },
            { text: "B", value: this.CategoriaB },
            { text: "C", value: this.CategoriaC },
            { text: "D", value: this.CategoriaD },
            { text: "E", value: this.CategoriaE },
            { text: "AB", value: this.CategoriaAB },
            { text: "AC", value: this.CategoriaAC },
            { text: "AD", value: this.CategoriaAD },
            { text: "AE", value: this.CategoriaAE }
        ];
    }
};

var EnumCategoriaHabilitacao = Object.freeze(new EnumEnumCategoriaHabilitacaoHelper());