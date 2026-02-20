var EnumTipoMontagemCarregamentoPedidoProdutoHelper = function () {
    this.Ambos = 0;
    this.ProdutosPalletsFechado = 1;
    this.ProdutosSemPalletsFechado = 2;
}

EnumTipoMontagemCarregamentoPedidoProdutoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoMontagemCarregamentoPedidoProduto.Ambos, value: this.Ambos },
            { text: Localization.Resources.Enumeradores.TipoMontagemCarregamentoPedidoProduto.ProdutosDePalletsFechado, value: this.ProdutosPalletsFechado },
            { text: Localization.Resources.Enumeradores.TipoMontagemCarregamentoPedidoProduto.ProdutosSemPalletsFechado, value: this.ProdutosSemPalletsFechado },
        ];
    }
}

var EnumTipoMontagemCarregamentoPedidoProduto = Object.freeze(new EnumTipoMontagemCarregamentoPedidoProdutoHelper());