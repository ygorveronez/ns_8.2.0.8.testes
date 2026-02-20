var EnumTipoDocumentoCreditoDebitoHelper = function () {
    this.Todos = "";
    this.Credito = 0;
    this.Debito = 1;
};

EnumTipoDocumentoCreditoDebitoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.CreditoDebito.Credito, value: this.Credito },
            { text: Localization.Resources.Enumeradores.CreditoDebito.Debito, value: this.Debito }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.CreditoDebito.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoDocumentoCreditoDebito = Object.freeze(new EnumTipoDocumentoCreditoDebitoHelper());
