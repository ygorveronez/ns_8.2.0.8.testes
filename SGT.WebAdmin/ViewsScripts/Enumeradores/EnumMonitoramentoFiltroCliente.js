var EnumMonitoramentoFiltroClienteHelper = function () {
    this.Nenhum = 0;
    this.EmAlvo = 1;
    this.ComColeta = 2;
    this.ComEntrega = 3;
    this.ComColetaOuEntrega = 4;
};

EnumMonitoramentoFiltroClienteHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Logistica.Monitoramento.Nenhum, value: this.Nenhum },
            { text: Localization.Resources.Logistica.Monitoramento.EmAlvo, value: this.EmAlvo },
            { text: Localization.Resources.Logistica.Monitoramento.ComColeta, value: this.ComColeta },
            { text: Localization.Resources.Logistica.Monitoramento.ComEntrega, value: this.ComEntrega },
            { text: Localization.Resources.Logistica.Monitoramento.ComColetaOuEntrega, value: this.ComColetaOuEntrega }
        ];
    }
}

var EnumMonitoramentoFiltroCliente = Object.freeze(new EnumMonitoramentoFiltroClienteHelper());