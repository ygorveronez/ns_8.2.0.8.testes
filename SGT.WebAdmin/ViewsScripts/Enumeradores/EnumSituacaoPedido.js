var EnumSituacaoPedidoHelper = function () {
    this.Todos = 0;
    this.Aberto = 1;
    this.Cancelado = 2;
    this.Finalizado = 3;
    this.AgAprovacao = 4;
    this.Rejeitado = 5;
    this.AutorizacaoPendente = 7;
    this.EmCotacao = 12;
};

EnumSituacaoPedidoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacaoPedido.Aberto, value: this.Aberto },
            { text: Localization.Resources.Enumeradores.SituacaoPedido.Cancelado, value: this.Cancelado },
            { text: Localization.Resources.Enumeradores.SituacaoPedido.Finalizado, value: this.Finalizado },
            { text: Localization.Resources.Enumeradores.SituacaoPedido.AgAprovacao, value: this.AgAprovacao },
            { text: Localization.Resources.Enumeradores.SituacaoPedido.Rejeitado, value: this.Rejeitado },
            { text: Localization.Resources.Enumeradores.SituacaoPedido.AutorizacaoPendente, value: this.AutorizacaoPendente },
            { text: Localization.Resources.Enumeradores.SituacaoPedido.EmCotacao, value: this.EmCotacao }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.SituacaoPedido.Todos, value: this.Todos }].concat(this.obterOpcoes());
    },
};

var EnumSituacaoPedido = Object.freeze(new EnumSituacaoPedidoHelper());