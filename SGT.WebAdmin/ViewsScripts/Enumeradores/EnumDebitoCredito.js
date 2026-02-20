var EnumDebitoCreditoHelper = function () {
    this.Todos = "";
    this.Debito = 1;
    this.Credito = 2;
};

EnumDebitoCreditoHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: "Débito", value: this.Debito },
            { text: "Crédito", value: this.Credito }
        ];
    },
    ObterDescricao: function (valor) {
        switch (valor) {
            case this.Credito:
                return "Crédito";
            case this.Debito:
                return "Débito";
            default:
                return "";
        }
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.ObterOpcoes());
    }
};

var EnumDebitoCredito = Object.freeze(new EnumDebitoCreditoHelper());