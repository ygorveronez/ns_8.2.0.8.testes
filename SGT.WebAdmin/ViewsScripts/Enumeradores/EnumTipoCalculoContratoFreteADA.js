var EnumTipoCalculoContratoFreteADAHelper = function () {
    this.Nenhum = "";
    this.DiasEntreAgendamentoPrevisaoSaida = 1;
};

EnumTipoCalculoContratoFreteADAHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Dias entre o Agendamento e a Previsão de Saída", value: this.DiasEntreAgendamentoPrevisaoSaida },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Nenhum", value: this.Nenhum }].concat(this.obterOpcoes());
    }
};

var EnumTipoCalculoContratoFreteADA = Object.freeze(new EnumTipoCalculoContratoFreteADAHelper());