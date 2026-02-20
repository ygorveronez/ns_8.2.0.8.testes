var EnumVerificarTipoDeCargaHelper = function () {
    this.NaoVerificar = 1;
    this.Algum = 2;
    this.Nenhum = 3;
}

EnumVerificarTipoDeCargaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Não verificar", value: this.NaoVerificar },
            { text: "Algum dos tipos", value: this.Algum },
            { text: "Nenhum dos tipos", value: this.Nenhum }
        ];
    }
}

var EnumVerificarTipoDeCarga = Object.freeze(new EnumVerificarTipoDeCargaHelper());