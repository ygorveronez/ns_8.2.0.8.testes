var EnumTipoConsolidacaoHelper = function () {
    this.NaoConsolida = 0;
    this.AutorizacaoEmissao = 1;
    this.PreCheckIn = 2;
}

EnumTipoConsolidacaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Não Consolida", value: this.NaoConsolida },
            { text: "Autorização Emissão", value: this.AutorizacaoEmissao },
            { text: "Pré CheckIn", value: this.PreCheckIn }
        ];
    },
}

var EnumTipoConsolidacao = Object.freeze(new EnumTipoConsolidacaoHelper());