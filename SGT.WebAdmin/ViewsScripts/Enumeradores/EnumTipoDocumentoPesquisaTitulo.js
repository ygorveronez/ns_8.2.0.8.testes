var EnumTipoDocumentoPesquisaTituloHelper = function () {
    this.Todos = "";
    this.Fatura = 0;
    this.Negociacao = 1;
    this.ContratoFrete = 2;
    this.DocumentoEntrada = 3;
    this.NotaFiscal = 4;
    this.ContratoFinanciamento = 5;
    this.Outros = 9;
};

EnumTipoDocumentoPesquisaTituloHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Fatura", value: this.Fatura },
            { text: "Negociação", value: this.Negociacao },
            { text: "Documento de Entrada", value: this.DocumentoEntrada },
            { text: "Nota Fiscal", value: this.NotaFiscal },
            { text: "Contrato de Frete", value: this.ContratoFrete },
            { text: "Contrato Financiamento", value: this.ContratoFinanciamento },
            { text: "Outros", value: this.Outros }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoDocumentoPesquisaTitulo = Object.freeze(new EnumTipoDocumentoPesquisaTituloHelper());