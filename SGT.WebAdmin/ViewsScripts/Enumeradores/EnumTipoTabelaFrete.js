var EnumTipoTabelaFreteHelper = function () {
    this.todas = 0;
    this.tabelaRota = 1;
    this.tabelaComissaoProduto = 2;
    this.tabelaCliente = 3;
    this.freteSemTabela = 4;
    this.tabelaSubContratacao = 5;

};

EnumTipoTabelaFreteHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoTabelaFrete.TabelaRota, value: this.tabelaRota },
            { text: Localization.Resources.Enumeradores.TipoTabelaFrete.TabelaComissaoProduto, value: this.tabelaComissaoProduto },
            { text: Localization.Resources.Enumeradores.TipoTabelaFrete.TabelaCliente, value: this.tabelaCliente },
            { text: Localization.Resources.Enumeradores.TipoTabelaFrete.FreteSemTabela, value: this.freteSemTabela },
            { text: Localization.Resources.Enumeradores.TipoTabelaFrete.TabelaSubContratacao, value: this.tabelaSubContratacao },
        ];
    },
    obterOpcoesTipoTabelaFrete: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoTabelaFrete.TabelaRota, value: this.tabelaRota },
            { text: Localization.Resources.Enumeradores.TipoTabelaFrete.TabelaComissaoProduto, value: this.tabelaComissaoProduto },
            { text: Localization.Resources.Enumeradores.TipoTabelaFrete.TabelaCliente, value: this.tabelaCliente },
        ];
    },
    obterOpcoesTipoTabelaFretePesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoTabelaFrete.Todas, value: this.todas }].concat(this.obterOpcoesTipoTabelaFrete());
    },
};

var EnumTipoTabelaFrete = Object.freeze(new EnumTipoTabelaFreteHelper());