var EnumTipoMontagemCarregamentoVrpHelper = function () {
    this.Nenhum = 0;
    this.VrpCapacity = 1;
    this.VrpTimeWindows = 2;
    this.SimuladorFrete = 3;
    this.Prioridades = 4;
}

EnumTipoMontagemCarregamentoVrpHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoMontagemCarregamentoVrp.Nenhum, value: this.Nenhum },
            { text: Localization.Resources.Enumeradores.TipoMontagemCarregamentoVrp.VrpCapacidade, value: this.VrpCapacity },
            { text: Localization.Resources.Enumeradores.TipoMontagemCarregamentoVrp.VrpJanelaDeTempo, value: this.VrpTimeWindows },
            { text: Localization.Resources.Enumeradores.TipoMontagemCarregamentoVrp.SimuladorFrete, value: this.SimuladorFrete },
            { text: Localization.Resources.Enumeradores.TipoMontagemCarregamentoVrp.Prioridades, value: this.Prioridades }
        ];
    }
}

var EnumTipoMontagemCarregamentoVrp = Object.freeze(new EnumTipoMontagemCarregamentoVrpHelper());





var EnumTipoResumoCarregamentoHelper = function () {
    this.SeparacaoProdutos = 0;
    this.ModeloCargas = 1;
    this.ModeloValorFrete = 2;
}

EnumTipoResumoCarregamentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoResumoCarregamento.SeparacaoProdutos, value: this.SeparacaoProdutos },
            { text: Localization.Resources.Enumeradores.TipoResumoCarregamento.ModeloCargas, value: this.ModeloCargas },
            { text: Localization.Resources.Enumeradores.TipoResumoCarregamento.ModeloValorFrete, value: this.ModeloValorFrete }
        ];
    }
}

var EnumTipoResumoCarregamento = Object.freeze(new EnumTipoResumoCarregamentoHelper());





var EnumSimuladorFreteCriterioSelecaoTransportadorHelper = function () {
    this.Nenhum = 0;
    this.MenorValor = 1;
    this.MenorPrazoEntrega = 2;
}

EnumSimuladorFreteCriterioSelecaoTransportadorHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.SimuladorFreteCriterioSelecaoTransportador.Nenhum, value: this.Nenhum },
            { text: Localization.Resources.Enumeradores.SimuladorFreteCriterioSelecaoTransportador.MenorValor, value: this.MenorValor },
            { text: Localization.Resources.Enumeradores.SimuladorFreteCriterioSelecaoTransportador.MenorPrazoEntrega, value: this.MenorPrazoEntrega }
        ];
    }
}

var EnumSimuladorFreteCriterioSelecaoTransportador = Object.freeze(new EnumSimuladorFreteCriterioSelecaoTransportadorHelper());