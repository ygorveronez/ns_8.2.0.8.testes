var EnumTipoContabilizacaoHelper = function () {
    this.Todos = 0;
    this.Credito = 1;
    this.Debito = 2;
    this.NotaDebito = 3;
    this.ContasAPagar = 4;
    this.ContasAReceber = 5;
}

EnumTipoContabilizacaoHelper.prototype = {
    obterDescricao: function (valor) {
        switch (valor) {
            case this.Credito: return "Crédito";
            case this.Debito: return "Débito";
            case this.NotaDebito: return "Nota de Débito";
            case this.ContasAPagar: return "Contas a Pagar";
            case this.ContasAReceber: return "Contas a Receber";
            case this.Todos: return "Todos";
            default: return "";
        }
    },
    obterOpcoes: function () {
        return [
            { text: "Crédito", value: this.Credito },
            { text: "Débito", value: this.Debito },
            { text: "Nota de Débito", value: this.NotaDebito },
            { text: "Contas a Pagar", value: this.ContasAPagar },
            { text: "Contas a Receber", value: this.ContasAReceber }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumTipoContabilizacao = Object.freeze(new EnumTipoContabilizacaoHelper());