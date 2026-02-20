var EnumCondicaoLiberacaoGRHelper = function () {
    this.NaoAplicado = "";
    this.E = 1;
    this.Ou = 2;
};

EnumCondicaoLiberacaoGRHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "E", value: this.E },
            { text: "Ou", value: this.Ou }
        ];
    }
};

var EnumCondicaoLiberacaoGR = Object.freeze(new EnumCondicaoLiberacaoGRHelper());