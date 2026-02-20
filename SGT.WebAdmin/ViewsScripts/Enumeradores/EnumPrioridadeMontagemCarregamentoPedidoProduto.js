var EnumPrioridadeMontagemCarregamentoPedidoProdutoHelper = function () {
    this.CanalEntregaLinhaSeparacaoPedido = 0;
    this.LinhaSeparacaoCanalEntregaPedido = 1;
    this.CanalEntregaLinhaSeparacaoProduto = 2;
    this.LinhaSeparacaoCanalEntregaProduto = 3;
    this.EnderecoProdutoDataPedido = 4;
    this.CanalEntregaEnderecoProdutoDataPedido = 5;
}

EnumPrioridadeMontagemCarregamentoPedidoProdutoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.PrioridadeMontagemCarregamentoPedidoProduto.CanalEntregaLinhaSeparacaoPedido, value: this.CanalEntregaLinhaSeparacaoPedido },
            { text: Localization.Resources.Enumeradores.PrioridadeMontagemCarregamentoPedidoProduto.LinhaSeparacaoCanalEntregaPedido, value: this.LinhaSeparacaoCanalEntregaPedido },
            { text: Localization.Resources.Enumeradores.PrioridadeMontagemCarregamentoPedidoProduto.CanalEntregaLinhaSeparacaoProduto, value: this.CanalEntregaLinhaSeparacaoProduto },
            { text: Localization.Resources.Enumeradores.PrioridadeMontagemCarregamentoPedidoProduto.LinhaSeparacaoCanalEntregaProduto, value: this.LinhaSeparacaoCanalEntregaProduto },
            { text: Localization.Resources.Enumeradores.PrioridadeMontagemCarregamentoPedidoProduto.EnderecoProdutoDataPedido, value: this.EnderecoProdutoDataPedido },
            { text: Localization.Resources.Enumeradores.PrioridadeMontagemCarregamentoPedidoProduto.CanalEntregaEnderecoProdutoDataPedido, value: this.CanalEntregaEnderecoProdutoDataPedido },
        ];
    }
}

var EnumPrioridadeMontagemCarregamentoPedidoProduto = Object.freeze(new EnumPrioridadeMontagemCarregamentoPedidoProdutoHelper());


var EnumPrioridadeMontagemCarregamentoPedidoHelper = function () {
    this.CanalEntregaPrevisaoEntrega = 0;
    this.PrevisaoEntregaCanalEntrega = 1;
}

EnumPrioridadeMontagemCarregamentoPedidoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.PrioridadeMontagemCarregamentoPedidoProduto.CanalEntregaPrevisaoEntrega, value: this.CanalEntregaPrevisaoEntrega },
            { text: Localization.Resources.Enumeradores.PrioridadeMontagemCarregamentoPedidoProduto.PrevisaoEntregaCanalEntrega, value: this.PrevisaoEntregaCanalEntrega }
        ];
    }
}

var EnumPrioridadeMontagemCarregamentoPedido = Object.freeze(new EnumPrioridadeMontagemCarregamentoPedidoHelper());