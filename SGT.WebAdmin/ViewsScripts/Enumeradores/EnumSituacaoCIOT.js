var EnumSituacaoCIOTHelper = function () {
    this.Aberto = 0;
    this.Encerrado = 1;
    this.Cancelado = 2;
    this.AgIntegracao = 3;
    this.Pendencia = 4;
    this.PagamentoAutorizado = 5;
    this.AgLiberarViagem = 6;
};

EnumSituacaoCIOTHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: "Aberto", value: this.Aberto },
            { text: "Encerrado", value: this.Encerrado },
            { text: "Cancelado", value: this.Cancelado },
            { text: "Ag. Integração", value: this.AgIntegracao },
            { text: "Pendência", value: this.Pendencia },
            { text: "Pagamento Autorizado", value: this.PagamentoAutorizado },
            { text: "Ag. Liberar Viagem", value: this.AgLiberarViagem }
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: "" }].concat(this.ObterOpcoes());
    }
};

var EnumSituacaoCIOT = Object.freeze(new EnumSituacaoCIOTHelper());
