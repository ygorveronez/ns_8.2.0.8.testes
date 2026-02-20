const EnumSituacaoLoteLiberacaoComercialPedidoHelper = function () {
    this.Todos = 0;
    this.EmIntegracao = 1;
    this.Finalizado = 2;
    this.FalhaNaIntegracao = 3;
};

EnumSituacaoLoteLiberacaoComercialPedidoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Em Integração", value: this.EmIntegracao },
            { text: "Finalizado", value: this.Finalizado },
            { text: "Falha na Integração", value: this.FalhaNaIntegracao }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    },
    obterValorEnum: function (texto) {
        if (texto === "Em Integração") {
            return this.EmIntegracao;
        } else if (texto === "Finalizado") {
            return this.Finalizado;
        } else if (texto === "Falha na Integração") {
            return this.FalhaNaIntegracao;
        }
    }
};

const EnumSituacaoLoteLiberacaoComercialPedido = Object.freeze(new EnumSituacaoLoteLiberacaoComercialPedidoHelper());