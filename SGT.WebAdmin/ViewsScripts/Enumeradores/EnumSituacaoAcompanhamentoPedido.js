var EnumSituacaoAcompanhamentoPedidoHelper = function () {
    this.Todos = 0;
    this.AgColeta = 1;
    this.ColetaAgendada = 2;
    this.ColetaRejeitada = 3;
    this.EmTransporte = 4;
    this.ProblemaNoTransporte = 5;
    this.SaiuParaEntrega = 6;
    this.Entregue = 7;
    this.EntregaRejeitada = 8;
    this.EntregaParcial = 9;
};

EnumSituacaoAcompanhamentoPedidoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacaoAcompanhamentoPedido.EmTransporte, value: this.EmTransporte },
            { text: Localization.Resources.Enumeradores.SituacaoAcompanhamentoPedido.SaiuParaEntrega, value: this.SaiuParaEntrega },
            { text: Localization.Resources.Enumeradores.SituacaoAcompanhamentoPedido.EntregaRejeitada, value: this.EntregaRejeitada },
            { text: Localization.Resources.Enumeradores.SituacaoAcompanhamentoPedido.EntregaParcial, value: this.EntregaParcial },
            { text: Localization.Resources.Enumeradores.SituacaoAcompanhamentoPedido.AgColeta, value: this.AgColeta },
            { text: Localization.Resources.Enumeradores.SituacaoAcompanhamentoPedido.ColetaAgendada, value: this.ColetaAgendada },
            { text: Localization.Resources.Enumeradores.SituacaoAcompanhamentoPedido.ColetaRejeitada, value: this.ColetaRejeitada },
            { text: Localization.Resources.Enumeradores.SituacaoAcompanhamentoPedido.Entregue, value: this.Entregue },
            //{ text: "Problema no Transporte", value: this.ProblemaNoTransporte },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.SituacaoAcompanhamentoPedido.Todos, value: this.Todos }].concat(this.obterOpcoes());
    },
    obterOpcoesPortalCliente: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacaoAcompanhamentoPedido.TodosStatus, value: this.Todos },
            { text: Localization.Resources.Enumeradores.SituacaoAcompanhamentoPedido.AgExpedicao, value: this.AgColeta },
            { text: Localization.Resources.Enumeradores.SituacaoAcompanhamentoPedido.EmTransferencia, value: this.EmTransporte },
            { text: Localization.Resources.Enumeradores.SituacaoAcompanhamentoPedido.EmRota, value: this.SaiuParaEntrega },
            { text: Localization.Resources.Enumeradores.SituacaoAcompanhamentoPedido.Entregue, value: this.Entregue },
            { text: Localization.Resources.Enumeradores.SituacaoAcompanhamentoPedido.EntregaParcial, value: this.EntregaParcial },
            { text: Localization.Resources.Enumeradores.SituacaoAcompanhamentoPedido.Devolucao, value: this.EntregaRejeitada },
            { text: Localization.Resources.Enumeradores.SituacaoAcompanhamentoPedido.Ocorrencias, value: this.ProblemaNoTransporte },
        ];
    },
    obterDescricaoPortalCliente: function (val) {
        switch (val) {
            case this.AgColeta: return Localization.Resources.Enumeradores.SituacaoAcompanhamentoPedido.AgExpedicao;
            case this.EmTransporte: return Localization.Resources.Enumeradores.SituacaoAcompanhamentoPedido.EmTransferencia;
            case this.SaiuParaEntrega: return Localization.Resources.Enumeradores.SituacaoAcompanhamentoPedido.EmRota;
            case this.Entregue: return Localization.Resources.Enumeradores.SituacaoAcompanhamentoPedido.Entregue;
            case this.EntregaParcial: return Localization.Resources.Enumeradores.SituacaoAcompanhamentoPedido.EntregaParcial;
            case this.EntregaRejeitada: return Localization.Resources.Enumeradores.SituacaoAcompanhamentoPedido.Devolucao;
            case this.ProblemaNoTransporte: return Localization.Resources.Enumeradores.SituacaoAcompanhamentoPedido.ProblemaTransferencia;
            default: return "";
        }
    },
}

var EnumSituacaoAcompanhamentoPedido = Object.freeze(new EnumSituacaoAcompanhamentoPedidoHelper());