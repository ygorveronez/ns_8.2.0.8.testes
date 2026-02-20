var EnumTipoDocumentoModeloDeImpressaoHelper = function () {
    this.NotaDeCreditoDebito = 0;
    this.NotaDePagamento = 1;
};

EnumTipoDocumentoModeloDeImpressaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Nota de Crédito/Débito", value: this.NotaDeCreditoDebito },
            { text: "Nota de Pagamento", value: this.NotaDePagamento }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoDocumentoModeloDeImpressao = Object.freeze(new EnumTipoDocumentoModeloDeImpressaoHelper());
