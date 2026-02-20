var EnumMonitorarPosicaoAtualVeiculoHelper = function () {
    this.Todos = 1;
    this.ComMonitoramentoEmAndamento = 2;
    this.PossuiContratoDeFrete = 3;
    this.ComMonitoramentoEmAndamentoOuPossuiContratoDeFrete = 4;
}

EnumMonitorarPosicaoAtualVeiculoHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: "Todos os veículos", value: this.Todos },
            { text: "Com monitoramento em andamento", value: this.ComMonitoramentoEmAndamento },
            { text: "Possui contrato de frete", value: this.PossuiContratoDeFrete },
            { text: "Com monitoramento em andamento ou possui contrato de frete", value: this.ComMonitoramentoEmAndamentoOuPossuiContratoDeFrete }
        ];
    }
};

var EnumMonitorarPosicaoAtualVeiculo = Object.freeze(new EnumMonitorarPosicaoAtualVeiculoHelper());