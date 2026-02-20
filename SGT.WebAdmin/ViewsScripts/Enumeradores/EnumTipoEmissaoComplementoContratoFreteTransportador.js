var EnumTipoEmissaoComplementoContratoFreteTransportadorHelper = function () {
    this.PorTomador = 0;
    this.PorVeiculoEMotorista = 1;
    this.PorVeiculoDoContrato = 2;
    this.PortTomadorECarga = 3;
};

EnumTipoEmissaoComplementoContratoFreteTransportadorHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Por Tomador", value: this.PorTomador },
            { text: "Por Veículo do Contrato", value: this.PorVeiculoDoContrato },
            { text: "Por Veículo e Motorista", value: this.PorVeiculoEMotorista },
            { text: "Por Tomador e Carga", value: this.PortTomadorECarga}
        ];
    }
};

var EnumTipoEmissaoComplementoContratoFreteTransportador = Object.freeze(new EnumTipoEmissaoComplementoContratoFreteTransportadorHelper());
