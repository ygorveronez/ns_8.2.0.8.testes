var EnumCondicaoLicencaHelper = function () {
    this.NaoAplicado = "";
    this.E = 1;
    this.Ou = 2;
};

EnumCondicaoLicencaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "E", value: this.E },
            { text: "Ou", value: this.Ou }
        ];
    }
};

var EnumCondicaoLicenca = Object.freeze(new EnumCondicaoLicencaHelper());