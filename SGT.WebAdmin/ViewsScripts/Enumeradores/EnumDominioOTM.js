var EnumDominioOTMHelper = function () {
    this.SAO = 1;
};

EnumDominioOTMHelper.prototype = {
    obterOpcoes: function (opcaoSelecione) {
        var arrayOpcoes = [
            { text: "SAO", value: this.SAO },
        ];

        return arrayOpcoes;
    }
};

var EnumDominioOTM = Object.freeze(new EnumDominioOTMHelper());