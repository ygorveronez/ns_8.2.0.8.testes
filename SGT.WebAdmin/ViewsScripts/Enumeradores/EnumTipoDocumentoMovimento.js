var EnumTipoDocumentoMovimentoHelper = function () {
    this.Todos = 0;
    this.Manual = 1;
    this.NotaEntrada = 2;
    this.CTe = 3;
    this.Faturamento = 4;
    this.Recibo = 5;
    this.Pagamento = 6;
    this.Recebimento = 7;
    this.NotaSaida = 8;
    this.Outros = 9;
    this.Acerto = 10;
    this.ContratoFrete = 11;
    this.Cheque = 14;
};

EnumTipoDocumentoMovimentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Manual", value: this.Manual },
            { text: "Nota de Entrada", value: this.NotaEntrada },
            { text: "Documento Emitido", value: this.CTe },
            { text: "Faturamento", value: this.Faturamento },
            { text: "Recibo", value: this.Recibo },
            { text: "Pagamento", value: this.Pagamento },
            { text: "Recebimento", value: this.Recebimento },
            { text: "Nota de Saída", value: this.NotaSaida },
            { text: "Acerto de Viagem", value: this.Acerto },
            { text: "Contrato de Frete", value: this.ContratoFrete },
            { text: "Cheque", value: this.Cheque },
            { text: "Outros", value: this.Outros }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoDocumentoMovimento = Object.freeze(new EnumTipoDocumentoMovimentoHelper());