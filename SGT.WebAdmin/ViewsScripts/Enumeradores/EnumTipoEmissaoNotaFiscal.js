var EnumTipoEmissaoNotaFiscalHelper = function () {
    this.NaoEletronica = 0;
    this.Normal = 1;
    this.ContingenciaFSIA = 2;
    this.ContingenciaSCAN = 3;
    this.ContingenciaEPEC = 4;
    this.ContingenciaFSDA = 5;
    this.ContingenciaSVCAN = 6;
    this.ContingenciaSVCRS = 7;
    this.ContingenciaOffLine = 9;
};

EnumTipoEmissaoNotaFiscalHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Emissão normal", value: this.Normal },
            { text: "Contingência FS-IA", value: this.ContingenciaFSIA },
            { text: "Contingência SCAN", value: this.ContingenciaSCAN },
            { text: "Contingência DPEC", value: this.ContingenciaEPEC },
            { text: "Contingência FS-DA", value: this.ContingenciaFSDA },
            { text: "Contingência SVC-AN", value: this.ContingenciaSVCAN },
            { text: "Contingência SVC-RS", value: this.ContingenciaSVCRS },
            { text: "Contingência off-line da NFC-e", value: this.ContingenciaOffLine }
        ];
    }
};

var EnumTipoEmissaoNotaFiscal = Object.freeze(new EnumTipoEmissaoNotaFiscalHelper());