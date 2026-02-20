var EnumPesoConsideradoCargaHelper = function () {
    this.PesoLiquido = 0;
    this.PesoBruto = 1;
}

EnumPesoConsideradoCargaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Peso liquído", value: this.PesoLiquido },
            { text: "Peso bruto", value: this.PesoBruto }
        ];
    },
}

var EnumPesoConsideradoCarga = Object.freeze(new EnumPesoConsideradoCargaHelper());