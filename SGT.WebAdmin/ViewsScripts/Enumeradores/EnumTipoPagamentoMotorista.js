var EnumTipoPagamentoMotoristaHelper = function () {
    this.Todos = -1;
    this.Nenhum = 0;
    this.Diaria = 1;
    this.Adiantamento = 2;
    this.Terceiro = 3;
};

EnumTipoPagamentoMotoristaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Outro", value: this.Nenhum },
            { text: "Diária", value: this.Diaria },
            { text: "Adiantamento", value: this.Adiantamento },
            { text: "Terceiro", value: this.Terceiro }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [
            { text: "Todos", value: this.Todos },
            { text: "Outro", value: this.Nenhum },
            { text: "Diária", value: this.Diaria },
            { text: "Adiantamento", value: this.Adiantamento },
            { text: "Terceiro", value: this.Terceiro }
        ];
    }
};

var EnumTipoPagamentoMotorista = Object.freeze(new EnumTipoPagamentoMotoristaHelper());