var TipoAnexoMotoristaHelper = function () {
    this.Outros = 0;
    this.Cnh = 1;
}

TipoAnexoMotoristaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Outros", value: this.Outros },
            { text: "CNH", value: this.Cnh }
        ];
    }
}

var EnumTipoAnexoMotorista = Object.freeze(new TipoAnexoMotoristaHelper());