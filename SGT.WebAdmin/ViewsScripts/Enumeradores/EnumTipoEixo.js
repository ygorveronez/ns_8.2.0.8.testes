var EnumTipoEixoHelper = function () {
    this.Todos = "";
    this.Direcional = 1;
    this.Livre = 2;
    this.Tracao = 3;
};

EnumTipoEixoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Direcional", value: this.Direcional },
            { text: "Livre", value: this.Livre },
            { text: "Tração", value: this.Tracao }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoEixo = Object.freeze(new EnumTipoEixoHelper());