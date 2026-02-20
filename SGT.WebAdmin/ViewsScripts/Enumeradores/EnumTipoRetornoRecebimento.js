var EnumTipoRetornoRecebimentoHelper = function () {
    this.Todos = null;
    this.Recebimento = 1;
    this.Retorno = 2;
};

EnumTipoRetornoRecebimentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Recebimento", value: this.Recebimento },
            { text: "Retorno", value: this.Retorno }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(obterOpcoes());
    }
}

var EnumTipoRetornoRecebimento = Object.freeze(new EnumTipoRetornoRecebimentoHelper());