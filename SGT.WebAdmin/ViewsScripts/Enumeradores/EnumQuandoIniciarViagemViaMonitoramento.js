var EnumQuandoIniciarViagemViaMonitoramentoHelper = function () {
    this.AoChegarNaOrigem = 1;
    this.AoSairDaOrigem = 2;
    this.NoStatusViagemTransito = 3;
};

EnumQuandoIniciarViagemViaMonitoramentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            {
                value: this.AoChegarNaOrigem,
                text: "Ao chegar na origem"
            },{
                value: this.AoSairDaOrigem,
                text: "Ao sair da origem"
            }, {
                value: this.NoStatusViagemTransito,
                text: "No status de viagem \"Trânsito\""
            }
        ];
    }
}

var EnumQuandoIniciarViagemViaMonitoramento = Object.freeze(new EnumQuandoIniciarViagemViaMonitoramentoHelper());