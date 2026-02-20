var EnumSituacaoPagamentoEletronicoHelper = function () {
    this.Todos = 0;
    this.Pendente = 1;
    this.Gerado = 2;
};

EnumSituacaoPagamentoEletronicoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Pendente", value: this.Pendente },
            { text: "Gerado", value: this.Gerado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoPagamentoEletronico = Object.freeze(new EnumSituacaoPagamentoEletronicoHelper());