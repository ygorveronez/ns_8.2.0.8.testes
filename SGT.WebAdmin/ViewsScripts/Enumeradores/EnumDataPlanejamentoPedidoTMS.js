var EnumDataPlanejamentoPedidoTMSHelper = function () {
    this.DataCarregamento = 1;
    this.DataAgendamento = 2;
};

EnumDataPlanejamentoPedidoTMSHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Data de Carregamento", value: this.DataCarregamento },
            { text: "Data do Agendamento", value: this.DataAgendamento }
        ];
    }
};

var EnumDataPlanejamentoPedidoTMS = Object.freeze(new EnumDataPlanejamentoPedidoTMSHelper());