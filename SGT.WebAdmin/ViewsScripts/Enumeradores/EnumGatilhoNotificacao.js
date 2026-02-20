var EnumGatilhoNotificacaoHelper = function () {
    this.AdicionarAgendamento = 1;
    this.ConfirmarAgendamento = 2;
    this.AlterarHorario = 3;
    this.DescargaArmazemExterno = 4;
    this.CancelarAgendamento = 5;
    this.NoShowNaoComparecimento = 6;
    this.CargaDevolvida = 7;
    this.CargaEntregueParcial = 8;
    this.CargaDevolvidaParcial = 9;
    this.Desagendar = 10;
    this.ChegadaConfirmada = 11;
    this.SaidaConfirmada = 12;
    this.DescarregamentoFinalizado = 13;
    this.ReagendarAgendamento = 14;
};


EnumGatilhoNotificacaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Adicionar Agendamento", value: this.AdicionarAgendamento },
            { text: "Confirmar Agendamento", value: this.ConfirmarAgendamento },
            { text: "Alterar Horário", value: this.AlterarHorario },
            { text: "Reagendar o Agendamento", value: this.ReagendarAgendamento },
            { text: "Descarga Armazém Externo", value: this.DescargaArmazemExterno },
            { text: "Cancelar Agendamento", value: this.CancelarAgendamento },
            { text: "No Show / Não Comparecimento", value: this.NoShowNaoComparecimento },
            { text: "Carga Devolvida", value: this.CargaDevolvida },
            { text: "Carga Entregue Parcial", value: this.CargaEntregueParcial },
            { text: "Carga Devolvida Parcial", value: this.CargaDevolvidaParcial },
            { text: "Desagendar", value: this.Desagendar },
            { text: "Chegada Confirmada", value: this.ChegadaConfirmada },
            { text: "Saída Confirmada", value: this.SaidaConfirmada },
            { text: "Descarregamento Finalizado", value: this.DescarregamentoFinalizado }
        ];
    }
};

var EnumGatilhoNotificacao = Object.freeze(new EnumGatilhoNotificacaoHelper());
