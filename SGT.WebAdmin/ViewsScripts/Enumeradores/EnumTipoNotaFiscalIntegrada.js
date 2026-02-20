var EnumTipoNotaFiscalIntegrada = function () {
    this.Todos = 0;
    this.Faturamento = 1;
    this.RemessaPallet = 2;
    this.OrdemVenda = 3;
    this.RemessaVenda = 4;
};

EnumTipoNotaFiscalIntegrada.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Faturamento", value: this.Faturamento },
            { text: "Remessa Pallet", value: this.RemessaPallet },
            { text: "Ordem Venda", value: this.OrdemVenda },
            { text: "Remessa Venda", value: this.RemessaVenda }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    },
};

var EnumTipoNotaFiscalIntegrada = Object.freeze(new EnumTipoNotaFiscalIntegrada());