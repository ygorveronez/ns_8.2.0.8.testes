var EnumTipoDataCalculoParadaNoPrazoHelper = function () {
    this.DataEntradaNoRaio = 1;
    this.DataInicio = 2;
    this.DataConfirmacao = 3;
    this.DataPrevista = 4;
    this.DataAgendamento = 5;
}

EnumTipoDataCalculoParadaNoPrazoHelper.prototype = {
    obterDescricao: function (evento) {
        switch (evento) {
            case this.DataEntradaNoRaio: return "Data de Entrada no Raio do Cliente da Coleta/Entrega";
            case this.DataInicio: return "Data de Início Coleta/Entrega";
            case this.DataConfirmacao: return "Data da Confirmação da Coleta/Entrega";
            case this.DataPrevista: return "Data Prevista da Coleta/Entrega";
            case this.DataAgendamento: return "Data de Agendamento da Coleta/Entrega";
            default: return "";
        }
    },

    obterOpcoesDataRealizada: function () {
        return [
            { text: this.obterDescricao(this.DataEntradaNoRaio), value: this.DataEntradaNoRaio },
            { text: this.obterDescricao(this.DataInicio), value: this.DataInicio },
            { text: this.obterDescricao(this.DataConfirmacao), value: this.DataConfirmacao },
        ];
    },
    obterOpcoesDataPrevista: function () {
        return [
            { text: this.obterDescricao(this.DataPrevista), value: this.DataPrevista },
            { text: this.obterDescricao(this.DataAgendamento), value: this.DataAgendamento },
        ];
    },
};

var EnumTipoDataCalculoParadaNoPrazo = Object.freeze(new EnumTipoDataCalculoParadaNoPrazoHelper());