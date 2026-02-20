var EnumMonitoramentoTratativaAutomaticaHelper = function () {
    this.InicioViagem = 0;
    this.ConfirmacaoEntrega = 1;
    this.RetornoSinal = 2;
    this.SensorTemperaturaComProblema = 3;
    this.TendenciaDeEntregaAdiantada= 4;
}

EnumMonitoramentoTratativaAutomaticaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Início da Viagem", value: this.InicioViagem },
            { text: "Confirmação da Entrega", value: this.ConfirmacaoEntrega },
            { text: "Retorno do Sinal", value: this.RetornoSinal },
            { text: "Sensor de Temperatura com Problema", value: this.SensorTemperaturaComProblema },
            { text: "Tendência de entrega adiantada", value: this.TendenciaDeEntregaAdiantada },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Início da Viagem", value: this.InicioViagem}].concat(this.obterOpcoes());
    }
};

var EnumMonitoramentoTratativaAutomatica = Object.freeze(new EnumMonitoramentoTratativaAutomaticaHelper());