var EnumTipoEmissaoEstadoHelper = function () {
    this.Normal = "1";
    this.EPECPelaSVC = "4";
    this.ContingenciaFSDA = "5";
    this.SVCRS = "7";
    this.SVCSP = "9";
};

EnumTipoEmissaoEstadoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Emissão normal", value: this.Normal },
            { text: "EPEC pela SVC", value: this.EPECPelaSVC },
            { text: "Contingência FS-DA", value: this.ContingenciaFSDA },
            { text: "SVC-RS", value: this.SVCRS },
            { text: "SVC-SP", value: this.SVCSP }
        ];
    }
};

var EnumTipoEmissaoEstado = Object.freeze(new EnumTipoEmissaoEstadoHelper());