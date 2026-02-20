
var EnumTipoConfiguracaoNCPendenteHelper = function () {
    this.Reusmo = 1;
    this.Individual = 2;
};

EnumTipoConfiguracaoNCPendenteHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Resumo", value: this.Reusmo },
            { text: "Individual", value: this.Individual },
        ]
    },

};


var EnumTipoConfiguracaoNCPendente = Object.freeze(new EnumTipoConfiguracaoNCPendenteHelper());