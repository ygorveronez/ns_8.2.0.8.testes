var EnumPadraoTempoDiasMinutosHelper = function () {
    this.Minutos = 1;
    this.Dias = 2;
}

EnumPadraoTempoDiasMinutosHelper.prototype = {
    obterDescricao: function (origem) {
        switch (origem) {
            case this.Minutos: return "Minutos";
            case this.Dias: return "Dias";
            default: return "";
        }
    },
    obterOpcoes: function () {
        return [
            { text: "Minutos", value: this.Minutos },
            { text: "Dias", value: this.Dias }
        ];
    }
};

var EnumPadraoTempoDiasMinutos = Object.freeze(new EnumPadraoTempoDiasMinutosHelper());