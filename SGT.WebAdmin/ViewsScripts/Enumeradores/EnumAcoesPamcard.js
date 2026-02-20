var EnumAcoesPamcardHelper = function () {
    this.SomenteCompra = 0;
    this.ConsultaCompra = 1;
    this.SomenteConsulta = 2;
};

EnumAcoesPamcardHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Somente Compra", value: this.SomenteCompra },
            { text: "Consulta e Compra", value: this.ConsultaCompra },
            { text: "Somente Consulta", value: this.SomenteConsulta }
        ];
    }
};

var EnumAcoesPamcard = Object.freeze(new EnumAcoesPamcardHelper());