var EnumRotaFreteClasseHelper = function () {
    this.Todas = 0;
    this.Um = 1;
    this.Dois = 2;
    this.Tres = 3;
    this.Quatro = 4;
    this.Cinco = 5;
};

EnumRotaFreteClasseHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Classe 1", value: this.Um },
            { text: "Classe 2", value: this.Dois },
            { text: "Classe 3", value: this.Tres },
            { text: "Classe 4", value: this.Quatro },
            { text: "Classe 5", value: this.Cinco }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
}

var EnumRotaFreteClasse = Object.freeze(new EnumRotaFreteClasseHelper());