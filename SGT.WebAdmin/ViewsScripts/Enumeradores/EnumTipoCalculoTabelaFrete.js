var EnumTipoCalculoTabelaFreteHelper = function () {
    this.PorCarga = 0;
    this.PorPedido = 1;
    this.PorDocumentoEmitido = 2;
    this.PorMaiorValorPedido = 3;
    this.PorPedidosAgrupados = 4;
    this.PorMaiorValorPedidoAgrupados = 5;
    this.PorMaiorDistanciaPedidoAgrupados = 6;
};

EnumTipoCalculoTabelaFreteHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoCalculoTabelaFrete.PorCarga, value: this.PorCarga },
            { text: Localization.Resources.Enumeradores.TipoCalculoTabelaFrete.PorPedido, value: this.PorPedido },
            { text: Localization.Resources.Enumeradores.TipoCalculoTabelaFrete.PorDocumentoEmitido, value: this.PorDocumentoEmitido },
            { text: Localization.Resources.Enumeradores.TipoCalculoTabelaFrete.PorMaiorValorPedido, value: this.PorMaiorValorPedido },
            { text: Localization.Resources.Enumeradores.TipoCalculoTabelaFrete.PorPedidosAgrupados, value: this.PorPedidosAgrupados },
            { text: Localization.Resources.Enumeradores.TipoCalculoTabelaFrete.PorMaiorValorPedidoAgrupados, value: this.PorMaiorValorPedidoAgrupados },
            { text: Localization.Resources.Enumeradores.TipoCalculoTabelaFrete.PorMaiorDistanciaPedidoAgrupados, value: this.PorMaiorDistanciaPedidoAgrupados }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Fretes.TabelaFrete.todas, value: this.Todos }].concat(this.obterOpcoes());
    },
    obterDescricao: function(tipoCalculoTabelaFrete) {
        switch (tipoCalculoTabelaFrete) {
            case this.PorCarga:
                return Localization.Resources.Enumeradores.TipoCalculoTabelaFrete.PorCarga;
            case this.PorDocumentoEmitido:
                return Localization.Resources.Enumeradores.TipoCalculoTabelaFrete.PorDocumentoEmitido;
            case this.PorMaiorValorPedido:
                return Localization.Resources.Enumeradores.TipoCalculoTabelaFrete.PorMaiorValorPedido;
            case this.PorPedido:
                return Localization.Resources.Enumeradores.TipoCalculoTabelaFrete.PorPedido;
            case this.PorPedidosAgrupados:
                return Localization.Resources.Enumeradores.TipoCalculoTabelaFrete.PorPedidosAgrupados;
            case this.PorMaiorValorPedidoAgrupados:
                return Localization.Resources.Enumeradores.TipoCalculoTabelaFrete.PorMaiorValorPedidoAgrupados;
            case this.PorMaiorDistanciaPedidoAgrupados:
                return Localization.Resources.Enumeradores.TipoCalculoTabelaFrete.PorMaiorDistanciaPedidoAgrupados;
            default:
                return "";
        }
    }
};

var EnumTipoCalculoTabelaFrete = Object.freeze(new EnumTipoCalculoTabelaFreteHelper());