var EnumTipoOperacaoNotaFiscalHelper = function() {
    this.Todos = "";
    this.Entrada = 0;
    this.Saida = 1;
}

EnumTipoOperacaoNotaFiscalHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Entrada", value: this.Entrada },
            { text: "Saída", value: this.Saida },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumTipoOperacaoNotaFiscal = Object.freeze(new EnumTipoOperacaoNotaFiscalHelper());