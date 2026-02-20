var EnumTipoGatilhoPedidoOcorrenciaColetaEntregaHelper = function () {
    this.Todos = "";
    this.CriacaoPedido = 1;
    this.FinalizacaoEmissaoCarga = 2;
}

EnumTipoGatilhoPedidoOcorrenciaColetaEntregaHelper.prototype = {
    obterDescricao: function (tipo) {
        switch (tipo) {
            case this.CriacaoPedido: return Localization.Resources.Enumeradores.TipoGatilhoPedidoOcorrenciaColetaEntrega.CriacaoDoPedido;
            case this.FinalizacaoEmissaoCarga: return Localization.Resources.Enumeradores.TipoGatilhoPedidoOcorrenciaColetaEntrega.FinalizacaoDaEmissaoDaCarga;
            default: return "";
        }
    },
    obterOpcoes: function () {
        return [
            { text: this.obterDescricao(this.CriacaoPedido), value: this.CriacaoPedido },
            { text: this.obterDescricao(this.FinalizacaoEmissaoCarga), value: this.FinalizacaoEmissaoCarga },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoGatilhoPedidoOcorrenciaColetaEntrega.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumTipoGatilhoPedidoOcorrenciaColetaEntrega = Object.freeze(new EnumTipoGatilhoPedidoOcorrenciaColetaEntregaHelper());
