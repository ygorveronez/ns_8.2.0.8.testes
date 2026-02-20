var EnumTipoDataAlteracaoGatilhoHelper = function () {
    this.DataAgendamentoEntregaTransportador = 1;
}

EnumTipoDataAlteracaoGatilhoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Data de Agendamento de Entrega do Transportador", value: this.DataAgendamentoEntregaTransportador },
        ];
    }
}

var EnumTipoDataAlteracaoGatilho = Object.freeze(new EnumTipoDataAlteracaoGatilhoHelper());