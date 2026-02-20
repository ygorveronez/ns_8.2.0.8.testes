var EnumTipoRestricaoPalletModeloVeicularCargaHelper = function () {
    this.BloquearSomenteNumeroSuperior = 0;
    this.BloquearSomenteNumeroInferior = 1;
    this.NaoBloquear = 2;
};

EnumTipoRestricaoPalletModeloVeicularCargaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Bloquear Somente Número de Pallets Inferior", value: this.BloquearSomenteNumeroInferior },
            { text: "Bloquear Somente Número de Pallets Superior", value: this.BloquearSomenteNumeroSuperior },
            { text: "Não Bloquear", value: this.NaoBloquear }
        ];
    }
}

var EnumTipoRestricaoPalletModeloVeicularCarga = Object.freeze(new EnumTipoRestricaoPalletModeloVeicularCargaHelper());