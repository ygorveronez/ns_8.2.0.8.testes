var EnumTipoAplicacaoGatilhoTrackingHelper = function () {
    this.AplicarSempre = 0;
    this.Coleta  = 1;
    this.Entrega  = 2;
}

EnumTipoAplicacaoGatilhoTrackingHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aplicar Sempre", value: this.AplicarSempre },
            { text: "Coleta", value: this.Coleta },
            { text: "Entrega", value: this.Entrega },
        ];
    }
}

var EnumTipoAplicacaoGatilhoTracking = Object.freeze(new EnumTipoAplicacaoGatilhoTrackingHelper());