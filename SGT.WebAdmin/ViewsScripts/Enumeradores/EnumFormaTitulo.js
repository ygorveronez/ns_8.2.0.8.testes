var EnumFormaTituloHelper = function () {
    this.Todos = -1;
    this.Outros = 0;
    this.Boleto = 1;
    this.Deposito = 2;
    this.Cheque = 3;
    this.Cartao = 4;
    this.Dinheiro = 5;
    this.Financiamento = 6;
    this.DebitoAutomatico = 7;
    this.Transferencia = 8;
    this.CobrancaJudicial = 9;
    this.PagamentoSalario = 10;
    this.Pix = 11;
};

EnumFormaTituloHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.FormaTitulo.Outros, value: this.Outros },
            { text: Localization.Resources.Enumeradores.FormaTitulo.Boleto, value: this.Boleto },
            { text: Localization.Resources.Enumeradores.FormaTitulo.Deposito, value: this.Deposito },
            { text: Localization.Resources.Enumeradores.FormaTitulo.Cheque, value: this.Cheque },
            { text: Localization.Resources.Enumeradores.FormaTitulo.Cartao, value: this.Cartao },
            { text: Localization.Resources.Enumeradores.FormaTitulo.Dinheiro, value: this.Dinheiro },
            { text: Localization.Resources.Enumeradores.FormaTitulo.Financiamento, value: this.Financiamento },
            { text: Localization.Resources.Enumeradores.FormaTitulo.DebitoAutomatico, value: this.DebitoAutomatico },
            { text: Localization.Resources.Enumeradores.FormaTitulo.Transferencia, value: this.Transferencia },
            { text: Localization.Resources.Enumeradores.FormaTitulo.CobrancaJudicial, value: this.CobrancaJudicial },
            { text: Localization.Resources.Enumeradores.FormaTitulo.PagamentoSalario, value: this.PagamentoSalario },
            { text: Localization.Resources.Enumeradores.FormaTitulo.Pix, value: this.Pix }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.FormaTitulo.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumFormaTitulo = Object.freeze(new EnumFormaTituloHelper());