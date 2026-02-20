var EnumDestinoComplementoHelper = function () {
    this.Subcontratada = 0;
    this.FilialEmissora = 1;
};

EnumDestinoComplementoHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: "Subcontratada", value: this.Subcontratada },
            { text: "Filial Emissora", value: this.FilialEmissora }
        ];
    }
};

var EnumDestinoComplemento = Object.freeze(new EnumDestinoComplementoHelper());