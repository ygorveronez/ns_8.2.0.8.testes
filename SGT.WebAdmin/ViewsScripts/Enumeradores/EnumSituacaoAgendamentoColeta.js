var EnumSituacaoAgendamentoColetaHelper = function () {
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

EnumSituacaoAgendamentoColetaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aguardando Confirmação", value: this.AguardandoConfirmacao },
            { text: "Agendado", value: this.Agendado },
            { text: "Finalizado", value: this.Finalizado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    },
    obterOpcoesPesquisaComSituacoesCanceladas: function () {
        return this.obterOpcoesPesquisa().concat([
            { text: "Cancelado Pelo Fornecedor", value: this.Cancelado },
            { text: "Cancelado Pelo Embarcador", value: this.CanceladoEmbarcador },
            { text: "Não Comparecido", value: this.NaoComparecimento },
            { text: "Carga Devolvida", value: this.CargaDevolvida },
            { text: "Não Comparecido (Confirmado pelo Fornecedor)", value: this.NaoComparecimentoConfirmadoPeloFornecedor },
            { text: "Ag. Geração Senha", value: this.AguardandoGeracaoSenha },
            { text: "Ag. CT-es", value: this.AguardandoCTes }
        ]);
    },
    isCancelado: function (situacao) {
        return [4, 5, 6, 7, 8].includes(situacao);
    }
}

var EnumSituacaoAgendamentoColeta = Object.freeze(new EnumSituacaoAgendamentoColetaHelper());
