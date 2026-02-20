var EnumEtapaFluxoCompraHelper = function () {
    this.Todos = 0;
    this.Requisicao = 1;
    this.AprovacaoRequisicao = 2;
    this.Cotacao = 3;
    this.RetornoCotacao = 4;
    this.OrdemCompra = 5;
    this.AprovacaoOrdemCompra = 6;
    this.RecebimentoProduto = 7;
};

EnumEtapaFluxoCompraHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Requisição", value: this.Requisicao },
            { text: "Aprovação da Requisição", value: this.AprovacaoRequisicao },
            { text: "Cotação", value: this.Cotacao },
            { text: "Retorno da Cotação", value: this.RetornoCotacao },
            { text: "Ordem de Compra", value: this.OrdemCompra },
            { text: "Aprovação Ordem de Compra", value: this.AprovacaoOrdemCompra },
            { text: "Recebimento do Produto", value: this.RecebimentoProduto },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumEtapaFluxoCompra = Object.freeze(new EnumEtapaFluxoCompraHelper());