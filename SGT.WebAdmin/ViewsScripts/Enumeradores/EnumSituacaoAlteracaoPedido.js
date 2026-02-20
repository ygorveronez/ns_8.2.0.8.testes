var EnumSituacaoAlteracaoPedidoHelper = function () {
    this.Todas = "";
    this.AguardandoAprovacao = 1;
    this.AguardandoAprovacaoTransportador = 2;
    this.Aprovada = 3;
    this.Reprovada = 4;
}

EnumSituacaoAlteracaoPedidoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aguardando Aprovação", value: this.AguardandoAprovacao },
            { text: "Aguardando Aprovação do Transportador", value: this.AguardandoAprovacaoTransportador },
            { text: "Aprovada", value: this.Aprovada },
            { text: "SemRegraAprovacao", value: this.SemRegraAprovacao },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoAlteracaoPedido = Object.freeze(new EnumSituacaoAlteracaoPedidoHelper());