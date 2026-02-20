const EnumDataEsperadaColetaEntregaTrizyHelper = function () {
    this.DataPrevisao = 0;
    this.DataAgendamento = 1;
    this.DataAgendamentoTransportador = 2;
};

EnumDataEsperadaColetaEntregaTrizyHelper.prototype = {
    obterOpcoes: function () {
        const opcoes = [];

        opcoes.push({ text: this.obterDescricao(this.DataPrevisao), value: this.DataPrevisao });
        opcoes.push({ text: this.obterDescricao(this.DataAgendamento), value: this.DataAgendamento });
        opcoes.push({ text: this.obterDescricao(this.DataAgendamentoTransportador), value: this.DataAgendamentoTransportador });

        return opcoes;
    },
    obterDescricao: function (tipo) {
        switch (tipo) {
            case this.DataPrevisao: return "Data Previsão";
            case this.DataAgendamento: return "Data de Agendamento";
            case this.DataAgendamentoTransportador: return "Data de Agendamento do Transportador";
            default: return "";
        }
    }
};

const EnumDataEsperadaColetaEntregaTrizy = Object.freeze(new EnumDataEsperadaColetaEntregaTrizyHelper());
