var EnumFormaPagamentoMotoristaHelper = function () {
    this.Nenhum = 0;
    this.Carga = 1;
    this.Descarga = 2;
};

EnumFormaPagamentoMotoristaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Carga", value: this.Carga },
            { text: "Descarga", value: this.Descarga }
        ];
    }
};

var EnumFormaPagamentoMotorista = Object.freeze(new EnumFormaPagamentoMotoristaHelper());