var EnumSituacaoCargaJanelaDescarregamentoHelper = function () {
    this.Todas = "";
    this.AguardandoConfirmacaoAgendamento = 1;
    this.AguardandoDescarregamento = 2;
    this.DescarregamentoFinalizado = 3;
    this.NaoComparecimento = 4;
    this.CargaDevolvida = 5;
    this.NaoComparecimentoConfirmadoPeloFornecedor = 6;
    this.AguardandoGeracaoSenha = 7;
    this.CargaDevolvidaParcialmente = 8;
    this.ChegadaConfirmada = 9;
    this.SaidaVeiculoConfirmada = 10;
    this.EntregaParcialmente = 11;
    this.Cancelado = 12;
    this.ValidacaoFiscal = 13;
    this.Nucleo = 14;
};

EnumSituacaoCargaJanelaDescarregamentoHelper.prototype = {
    isSituacaoDescarregamentoFinalizado: function (situacao) {
        return (
            situacao == this.DescarregamentoFinalizado ||
            situacao == this.NaoComparecimento ||
            situacao == this.CargaDevolvida ||
            situacao == this.NaoComparecimentoConfirmadoPeloFornecedor
        );
    },
    obterOpcoes: function (exibirOpcaoCancelado) {
        var opcoes = [
            { text: "Ag. Confirmação de Agendamento", value: this.AguardandoConfirmacaoAgendamento },
            { text: "Ag. Descarregamento", value: this.AguardandoDescarregamento },
            { text: "Descarregamento Finalizado", value: this.DescarregamentoFinalizado },
            { text: "Não Comparecido", value: this.NaoComparecimento },
            { text: "Carga Devolvida", value: this.CargaDevolvida },
            { text: "Não Comparecido (Confirmado pelo Fornecedor)", value: this.NaoComparecimentoConfirmadoPeloFornecedor },
            { text: "Ag. Geração Senha", value: this.AguardandoGeracaoSenha },
            { text: "Carga devolvida parcialmente", value: this.CargaDevolvidaParcialmente },
            { text: "Chegada Confirmada", value: this.ChegadaConfirmada },
            { text: "Saída do Veículo Confirmada", value: this.SaidaVeiculoConfirmada },
            { text: "Entrega Parcialmente", value: this.EntregaParcialmente },
            { text: "Validação Fiscal", value: this.ValidacaoFiscal },
            { text: "Validação Núcleo", value: this.Nucleo },
        ];

        if (exibirOpcaoCancelado)
            opcoes.concat({ text: "Cancelado", value: this.Cancelado });

        return opcoes;
    },
    obterOpcoesRelatorioQuantidadeDescarga: function () {
        return this.obterOpcoes().concat({ text: "Total de Cargas", value: 9999 });
    },
    obterOpcoesPesquisa: function (exibirOpcaoCancelado) {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes(exibirOpcaoCancelado));
    }
}

var EnumSituacaoCargaJanelaDescarregamento = Object.freeze(new EnumSituacaoCargaJanelaDescarregamentoHelper());
