var EnumTipoLiberacaoCargaJanelaCarregamentoHelper = function () {
    this.Todos = "";
    this.Normal = 1;
    this.Cotacao = 2;
};

EnumTipoLiberacaoCargaJanelaCarregamentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Cotação", value: this.Cotacao },
            { text: "Normal", value: this.Normal }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    },
}

var EnumTipoLiberacaoCargaJanelaCarregamento = Object.freeze(new EnumTipoLiberacaoCargaJanelaCarregamentoHelper());
