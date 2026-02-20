var EnumStatusDocumentoTransporteHelper = function () {
    this.OK = 1;
    this.NAOOK = 0;
 
}

EnumStatusDocumentoTransporteHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "OK", value: this.OK },
            { text: "NÃO OK", value: this.NAOOK }
        ];
    }
}

var EnumStatusDocumentoTransporte = Object.freeze(new EnumStatusDocumentoTransporteHelper());