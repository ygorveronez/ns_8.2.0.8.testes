var EnumTipoComprovanteSaidaHelper = function () {
    this.ComprovanteSaida = 0;
    this.RomaneioCarregamento = 1;
};

EnumTipoComprovanteSaidaHelper.prototype = {
    obterOpcoes: function () {
        return [
            {
                value: this.ComprovanteSaida,
                text: "Comprovante de Saída"
            }, {
                value: this.RomaneioCarregamento,
                text: "Romaneio de Carregamento"
            }
        ];
    }
}

var EnumTipoComprovanteSaida = Object.freeze(new EnumTipoComprovanteSaidaHelper());
