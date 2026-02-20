var EnumTipoRomaneioHelper = function () {
    this.Padrao = 1;
    this.Obramax = 2;
};

EnumTipoRomaneioHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Padrão", value: this.Padrao },
            { text: "Obramax", value: this.Obramax }
        ];
    },
}

var EnumTipoRomaneio = Object.freeze(new EnumTipoRomaneioHelper());