var EnumAcaoAoFinalizarMonitoramentoHelper = function () {
    this.Nenhuma = 1;
    this.IniciarProximoMonitoramentoAgendadoPorDataCriacao = 2;
    this.IniciarProximoMonitoramentoAgendadoPorDataCarregamentoCarga = 3;
};

EnumAcaoAoFinalizarMonitoramentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            {
                value: this.Nenhuma,
                text: "Nenhuma"
            },{
                value: this.IniciarProximoMonitoramentoAgendadoPorDataCriacao,
                text: "Iniciar próximo monitoramento agendado por data de criação"
            }, {
                value: this.IniciarProximoMonitoramentoAgendadoPorDataCarregamentoCarga,
                text: "Iniciar próximo monitoramento agendado por data de carregamento da carga"
            }
        ];
    }
}

var EnumAcaoAoFinalizarMonitoramento = Object.freeze(new EnumAcaoAoFinalizarMonitoramentoHelper());
