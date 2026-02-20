var EnumNivelInfracaoTransitoHelper = function () {
    this.Todos = "";
    this.Leve = 0;
    this.Media = 1;
    this.Grave = 2;
    this.Gravissima = 3;
}

EnumNivelInfracaoTransitoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Leve", value: this.Leve },
            { text: "Média", value: this.Media },
            { text: "Grave", value: this.Grave },
            { text: "Gravíssima", value: this.Gravissima }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumNivelInfracaoTransito = Object.freeze(new EnumNivelInfracaoTransitoHelper());