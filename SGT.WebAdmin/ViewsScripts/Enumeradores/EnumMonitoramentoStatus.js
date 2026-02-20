var EnumMonitoramentoStatusHelper = function () {
    this.Todas = 99;
    this.Aguardando = 0;
    this.Iniciado = 1;
    this.Finalizado = 2;
    this.Cancelado = 3;
};

EnumMonitoramentoStatusHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Logistica.Monitoramento.Agendado, value: this.Aguardando },
            { text: Localization.Resources.Logistica.Monitoramento.EmMonitoramento, value: this.Iniciado },
            { text: Localization.Resources.Logistica.Monitoramento.Finalizado, value: this.Finalizado },
            { text: Localization.Resources.Logistica.Monitoramento.Cancelado, value: this.Cancelado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Logistica.Monitoramento.Todas, value: this.Todas }].concat(this.obterOpcoes());
    }
}

var EnumMonitoramentoStatus = Object.freeze(new EnumMonitoramentoStatusHelper());