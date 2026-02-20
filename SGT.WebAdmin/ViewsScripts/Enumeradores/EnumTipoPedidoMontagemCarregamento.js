var EnumTipoPedidoMontagemCarregamentoHelper = function () {
    this.Card = 0;
    this.Tabela = 1;
}

EnumTipoPedidoMontagemCarregamentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoPedidoMontagemCarregamento.Card, value: this.Card },
            { text: Localization.Resources.Enumeradores.TipoPedidoMontagemCarregamento.Tabela, value: this.Tabela }
        ];
    }
}

var EnumTipoPedidoMontagemCarregamento = Object.freeze(new EnumTipoPedidoMontagemCarregamentoHelper());