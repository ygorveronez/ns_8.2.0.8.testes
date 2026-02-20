var EnumSituacaoAgendamentoAgendaCanceladaHelper = function () {
    this.Todas = "";
    this.AguardandoConfirmacao = 1;
    this.Agendado = 2;
    this.Finalizado = 3;
    this.Cancelado = 4;
    this.CanceladoEmbarcador = 5;
    this.NaoComparecimento = 6;
    this.CargaDevolvida = 7;
    this.NaoComparecimentoConfirmadoPeloFornecedor = 8;
    this.AguardandoGeracaoSenha = 9;
    this.AguardandoCTes = 10;
};

EnumSituacaoAgendamentoAgendaCanceladaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aguardando Confirmação", value: this.AguardandoConfirmacao },
            { text: "Agendado", value: this.Agendado },
            { text: "Finalizado", value: this.Finalizado },
            { text: "Cancelada pelo Fornecedor", value: this.Cancelado },
            { text: "Cancelada pelo Embarcador", value: this.CanceladoEmbarcador },
            { text: "Não Comparecido", value: this.NaoComparecimento },
            { text: "Carga Devolvida", value: this.CargaDevolvida },
            { text: "Cancelada pelo Fornecedor fora do prazo (suscetível a multa de No Show)", value: this.NaoComparecimentoConfirmadoPeloFornecedor },
            { text: "Aguardando Geração Senha", value: this.AguardandoGeracaoSenha },
            { text: "Aguardando CT-es", value: this.AguardandoCTes },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
}

var EnumSituacaoAgendamentoAgendaCancelada = Object.freeze(new EnumSituacaoAgendamentoAgendaCanceladaHelper());
