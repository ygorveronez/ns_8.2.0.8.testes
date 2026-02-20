var EnumTipoPagamentoHelper = function () {
    this.Todos = -1;
    this.Pago = 0;
    this.A_Pagar = 1;
    this.Outros = 2;
};

EnumTipoPagamentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoPagamento.Pago, value: this.Pago },
            { text: Localization.Resources.Enumeradores.TipoPagamento.A_Pagar, value: this.A_Pagar },
            { text: Localization.Resources.Enumeradores.TipoPagamento.Outros, value: this.Outros }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoPagamento.Todos, value: this.Todos },
            { text: Localization.Resources.Enumeradores.TipoPagamento.Pago, value: this.Pago },
            { text: Localization.Resources.Enumeradores.TipoPagamento.A_Pagar, value: this.A_Pagar },
            { text: Localization.Resources.Enumeradores.TipoPagamento.Outros, value: this.Outros }
        ];
    }
};

var EnumTipoPagamento = Object.freeze(new EnumTipoPagamentoHelper());