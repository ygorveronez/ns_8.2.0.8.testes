var EnumTipoPedidoVinculadoHelper = function () {
    this.Todos = 0;
    this.Normal = 1;
    this.Subcontratacao = 2;
    this.EncaixeSubContratacao = 3;
    this.EncaixePedidoSubContratacao = 4;
    this.Transbordo = 5;
    this.ColetaAnterior = 6;
};

EnumTipoPedidoVinculadoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoPedidoVinculado.Normal, value: this.Normal },
            { text: Localization.Resources.Enumeradores.TipoPedidoVinculado.Subcontratacao, value: this.Subcontratacao },
            { text: Localization.Resources.Enumeradores.TipoPedidoVinculado.EncaixeSubContratacao, value: this.EncaixeSubContratacao },
            { text: Localization.Resources.Enumeradores.TipoPedidoVinculado.EncaixePedidoSubContratacao, value: this.EncaixePedidoSubContratacao },
            { text: Localization.Resources.Enumeradores.TipoPedidoVinculado.Transbordo, value: this.Transbordo },
            { text: Localization.Resources.Enumeradores.TipoPedidoVinculado.ColetaAnterior, value: this.ColetaAnterior }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumTipoPedidoVinculado = Object.freeze(new EnumTipoPedidoVinculadoHelper());
