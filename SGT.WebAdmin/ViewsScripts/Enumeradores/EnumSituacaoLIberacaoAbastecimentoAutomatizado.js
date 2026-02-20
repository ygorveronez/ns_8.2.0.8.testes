var EnumSituacaoLiberacaoAbastecimentoAutomatizadoHelper = function () {
    this.Pendente = 0;
    this.PendenteReserva = 1;
    this.PendenteAutorizacao = 2;
    this.AgRetornoAbastecimento = 3;
    this.Finalizado = 4;
    this.ProblemaAbastecimento = 5;
    this.AgRetornoReserva = 6;
    this.AgRetornoAutorizacao = 7;
    this.Autorizado = 8;
};

EnumSituacaoLiberacaoAbastecimentoAutomatizadoHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: "Pendente", value: this.Pendente },
            { text: "Pendente Reserva", value: this.PendenteReserva },
            { text: "Pendente Autorização", value: this.PendenteAutorizacao },
            { text: "Em abastecimento", value: this.AgRetornoAbastecimento },
            { text: "Finalizado", value: this.Finalizado },
            { text: "Problema no abastecimento", value: this.ProblemaAbastecimento },
            { text: "Aguardando Retorno Reserva", value: this.AgRetornoReserva },
            { text: "Aguardando Retorno Autorização", value: this.AgRetornoAutorizacao },
            { text: "Autorizado", value: this.Autorizado }

        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: "" }].concat(this.ObterOpcoes());
    }
};

var EnumSituacaoLiberacaoAbastecimentoAutomatizado = Object.freeze(new EnumSituacaoLiberacaoAbastecimentoAutomatizadoHelper());