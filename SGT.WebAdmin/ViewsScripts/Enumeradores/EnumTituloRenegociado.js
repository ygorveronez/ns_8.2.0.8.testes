var EnumTituloRenegociadoHelper = function () {
    this.Todos = 0;
    this.Sim = 1;
    this.Nao = 2;
};

EnumTituloRenegociadoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Sim", value: this.Sim },
            { text: "Não", value: this.Nao }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTituloRenegociado = Object.freeze(new EnumTituloRenegociadoHelper());