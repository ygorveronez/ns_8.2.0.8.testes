var EnumOrigemRateioDespesaVeiculoHelper = function () {
    this.Todos = 0;
    this.ContratoFinanciamento = 1;
    this.DocumentoEntrada = 2;
    this.Infracao = 3;
    this.MovimentoFinanceiro = 4;
    this.Manual = 5;
    this.Titulo = 6;
    this.PagamentoMotorista = 7;
};

EnumOrigemRateioDespesaVeiculoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Contrato de Financiamento", value: this.ContratoFinanciamento },
            { text: "Documento de Entrada", value: this.DocumentoEntrada },
            { text: "Infração", value: this.Infracao },
            { text: "Movimento Financeiro", value: this.MovimentoFinanceiro },
            { text: "Manual", value: this.Manual },
            { text: "Título", value: this.Titulo },
            { text: "Pagamento Motorista", value: this.PagamentoMotorista }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumOrigemRateioDespesaVeiculo = Object.freeze(new EnumOrigemRateioDespesaVeiculoHelper());