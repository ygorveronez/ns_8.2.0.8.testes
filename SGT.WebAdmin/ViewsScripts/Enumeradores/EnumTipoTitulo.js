var EnumTipoTituloHelper = function () {
    this.Todos = 0;
    this.AReceber = 1;
    this.APagar = 2;
};

EnumTipoTituloHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "A Receber", value: this.AReceber },
            { text: "A Pagar", value: this.APagar }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoTitulo = Object.freeze(new EnumTipoTituloHelper());