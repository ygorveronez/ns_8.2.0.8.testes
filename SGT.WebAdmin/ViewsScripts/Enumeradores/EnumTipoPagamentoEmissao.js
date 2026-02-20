var EnumTipoPagamentoEmissaoHelper = function () {
    this.Todos = "";
    this.Pago = 0;
    this.A_Pagar = 1;
    this.Outros = 2;
    this.UsarDaNotaFiscal = 99;
};

EnumTipoPagamentoEmissaoHelper.prototype = {
    obterOpcoes: function (defaultValue, defaultText) {
        var opcoes = [];

        if (!string.IsNullOrWhiteSpace(defaultValue) || !string.IsNullOrWhiteSpace(defaultText))
            opcoes.push({ value: defaultValue, text: defaultText });

        return opcoes.concat([
            { text: Localization.Resources.Enumeradores.TipoPagamentoEmissao.Pago, value: this.Pago },
            { text: Localization.Resources.Enumeradores.TipoPagamentoEmissao.APagar, value: this.A_Pagar },
            { text: Localization.Resources.Enumeradores.TipoPagamentoEmissao.Outros, value: this.Outros },
            { text: Localization.Resources.Enumeradores.TipoPagamentoEmissao.UsarDaNotaFiscal, value: this.UsarDaNotaFiscal },
        ]);
    },
    obterOpcoesPesquisa: function () {
        return [{ value: this.Todos, text: Localization.Resources.Enumeradores.TipoPagamentoEmissao.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoPagamentoEmissao = Object.freeze(new EnumTipoPagamentoEmissaoHelper());