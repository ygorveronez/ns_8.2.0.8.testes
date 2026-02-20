var EnumTipoQuebraRegraHelper = function () {
    this.Padrao = 0;
    this.ValePallet = 1;
};

EnumTipoQuebraRegraHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Padrão", value: this.Padrao },
            { text: "Vale Pallet", value: this.ValePallet },
        ];
    },
};

var EnumTipoQuebraRegra = Object.freeze(new EnumTipoQuebraRegraHelper());