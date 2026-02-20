var EnumDocumentosFiscaisTrizyHelper = function () {
    this.Nenhum = 0;
    this.CTe = 1;
    this.MDFe = 2;
    this.NFe = 3;
    this.CIOT = 4;
    this.VPO = 5;
};

EnumDocumentosFiscaisTrizyHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "CTe", value: this.CTe},
            { text: "MDFe", value: this.MDFe},
            //{ text: "NFe", value: this.NFe },
            { text: "CIOT", value: this.CIOT },
            { text: "VPO", value: this.VPO }
        ];
    }
};

var EnumDocumentosFiscaisTrizy = Object.freeze(new EnumDocumentosFiscaisTrizyHelper());