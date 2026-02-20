var EnumSituacaoSeparacaoPedidoHelper = function () {
    this.Todos = "";
    this.Aberto = 1;
    this.AguardandoIntegracao = 2;
    this.IntegracaoRejeitada = 3;
    this.Finalizada = 4;
    this.Cancelada = 5;
};

EnumSituacaoSeparacaoPedidoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aberto", value: this.Aberto },
            { text: "Aguardando Integração", value: this.AguardandoIntegracao },
            { text: "Integração Rejeitada", value: this.IntegracaoRejeitada },
            { text: "Finalizada", value: this.Finalizada },
            { text: "Cancelada", value: this.Cancelada }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoSeparacaoPedido = Object.freeze(new EnumSituacaoSeparacaoPedidoHelper());