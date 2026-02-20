var EnumSituacaoChamadoTMSHelper = function () {
    this.Todos = 0;
    this.Aberto = 1;
    this.EmAnalise = 2;
    this.AguardandoAutorizacao = 3;
    this.LiberadaOcorrencia = 4;
    this.PagamentoNaoAutorizado = 5;
    this.Finalizado = 6;
    this.Cancelado = 7;
};

EnumSituacaoChamadoTMSHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aberto", value: this.Aberto },
            { text: "Em Análise", value: this.EmAnalise },
            { text: "Aguardando Autorização", value: this.AguardandoAutorizacao },
            { text: "Liberado para Ocorrência", value: this.LiberadaOcorrencia },
            { text: "Pagamento não Autorizado", value: this.PagamentoNaoAutorizado },
            { text: "Finalizado", value: this.Finalizado },
            { text: "Cancelado", value: this.Cancelado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoChamadoTMS = Object.freeze(new EnumSituacaoChamadoTMSHelper());