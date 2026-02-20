var EnumTipoObtencaoCNPJTransportadoraHelper = function () {
    this.Carga = 1;
    this.VeiculoTracao = 2;
}

EnumTipoObtencaoCNPJTransportadoraHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Carga", value: this.Carga },
            { text: "Veículo Tração", value: this.VeiculoTracao }
        ];
    }
}

var EnumTipoObtencaoCNPJTransportadora = Object.freeze(new EnumTipoObtencaoCNPJTransportadoraHelper());