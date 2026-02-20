var EnumTipoMovimentoExportacaoHelper = function () {
    this.Emissao = 1;
    this.Cancelamento = 2;
    this.BaixaTituloReceber = 3;
    this.AcrescimoDescontoBaixaTituloReceber = 4;
    this.CancelamentoBaixaTituloReceber = 5;
    this.CancelamentoAcrescimoDescontoBaixaTituloReceber = 6;
    this.AprovacaoContratoFrete = 7;
    this.ReversaoContratoFrete = 8;
    this.PagamentoContratoFrete = 9;
    this.ReversaoPagamentoContratoFrete = 10;
    this.AcrescimoDescontoFatura = 11;
    this.CancelamentoAcrescimoDescontoFatura = 12;
};

EnumTipoMovimentoExportacaoHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { value: this.Emissao, text: "Emissão" },
            { value: this.Cancelamento, text: "Cancelamento" },
            { value: this.BaixaTituloReceber, text: "Baixa de Título a Receber" },
            { value: this.CancelamentoBaixaTituloReceber, text: "Cancelamento de Baixa de Título a Receber" },
            { value: this.AcrescimoDescontoBaixaTituloReceber, text: "Acréscimo/Desconto na Baixa de Título a Receber" },
            { value: this.CancelamentoAcrescimoDescontoBaixaTituloReceber, text: "Cancelamento do Acréscimo/Desconto na Baixa de Título a Receber" },
            { value: this.AprovacaoContratoFrete, text: "Aprovação do Contrato de Frete" },
            { value: this.ReversaoContratoFrete, text: "Reversão/Cancelamento do Contrato de Frete" },
            { value: this.PagamentoContratoFrete, text: "Pagamento do Contrato de Frete" },
            { value: this.ReversaoPagamentoContratoFrete, text: "Reversão do Pagamento do Contrato de Frete" },
            { value: this.AcrescimoDescontoFatura, text: "Acréscimo/Desconto na Fatura" },
            { value: this.CancelamentoAcrescimoDescontoFatura, text: "Cancelamento no Acréscimo/Desconto da Fatura" }
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ value: "", text: "Todos" }].concat(this.ObterOpcoes());
    }
};

var EnumTipoMovimentoExportacao = Object.freeze(new EnumTipoMovimentoExportacaoHelper());