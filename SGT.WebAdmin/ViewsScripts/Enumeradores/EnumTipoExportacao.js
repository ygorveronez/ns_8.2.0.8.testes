var EnumTipoExportacaoHelper = function () {
    this.Todos = "";
    this.Debito = 1;
    this.Credito = 2;
    this.ContasAPagar = 3;
    this.ContasAReceber = 4;
    this.NotaDebito = 5;
    this.NotaCredito = 6;
};

EnumTipoExportacaoHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: "Débito", value: this.Debito },
            { text: "Crédito", value: this.Credito },
            { text: "Contas a Pagar", value: this.ContasAPagar },
            { text: "Contas a Receber", value: this.ContasAReceber },
            { text: "Nota de Débito", value: this.NotaDebito },
            { text: "Nota de Crédito", value: this.NotaCredito }
        ];
    },
    ObterDescricao: function (valor) {
        switch (valor) {
            case this.Credito:
                return "Crédito";
            case this.Debito:
                return "Débito";
            case this.ContasAPagar:
                return "Contas a Pagar";
            case this.ContasAReceber:
                return "Contas a Receber";
            case this.NotaDebito:
                return "Nota de Débito";
            case this.NotaCredito:
                return "Nota de Crédito";
            default:
                return "";
        }
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.ObterOpcoes());
    }
};

var EnumTipoExportacao = Object.freeze(new EnumTipoExportacaoHelper());