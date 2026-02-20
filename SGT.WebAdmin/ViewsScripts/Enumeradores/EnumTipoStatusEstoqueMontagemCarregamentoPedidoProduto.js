var EnumTipoStatusEstoqueMontagemCarregamentoPedidoProdutoHelper = function () {
    this.Ambos = 0;
    this.EstoqueParcial = 1;
    this.EstoqueTotal = 2;
}

EnumTipoStatusEstoqueMontagemCarregamentoPedidoProdutoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoStatusEstoqueMontagemCarregamentoPedidoProduto.Ambos, value: this.Ambos },
            { text: Localization.Resources.Enumeradores.TipoStatusEstoqueMontagemCarregamentoPedidoProduto.EstoqueParcial, value: this.EstoqueParcial },
            { text: Localization.Resources.Enumeradores.TipoStatusEstoqueMontagemCarregamentoPedidoProduto.EstoqueTotal, value: this.EstoqueTotal },
        ];
    }
}

var EnumTipoStatusEstoqueMontagemCarregamentoPedidoProduto = Object.freeze(new EnumTipoStatusEstoqueMontagemCarregamentoPedidoProdutoHelper());