var TipoAnexoVeiculoHelper = function () {
    this.Outros = 0;
    this.Crlv = 1;
}

TipoAnexoVeiculoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Outros", value: this.Outros },
            { text: "CRLV", value: this.Crlv }
        ];
    }
}

var EnumTipoAnexoVeiculo = Object.freeze(new TipoAnexoVeiculoHelper());