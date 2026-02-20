var EnumTipoImpressaoPedidoHelper = function () {
    this.AutorizacaoCarregamento = 1;
    this.AutorizacaoEntrega = 2;
    this.OrdemServicoOrdemColeta = 3;
    this.Simplificada = 4;
    this.AutorizacaoCarregamento_v2 = 5;
};

EnumTipoImpressaoPedidoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Autorização de Carregamento", value: this.AutorizacaoCarregamento },
            { text: "Autorização de Entrega", value: this.AutorizacaoEntrega },
            { text: "Ordem de Serviço", value: this.OrdemServicoOrdemColeta },
            { text: "Simplificada", value: this.Simplificada },
            { text: "Autorização de Carregamento v2", value: this.AutorizacaoCarregamento_v2 },
        ];
    }
};

var EnumTipoImpressaoPedido = Object.freeze(new EnumTipoImpressaoPedidoHelper());