var EnumSituacaoPneuHelper = function () {
    this.Todos = "";
    this.Incompleto = 1;
    this.Completo = 2;
};

EnumSituacaoPneuHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Incompleto", value: this.Incompleto },
            { text: "Completo", value: this.Completo }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoPneu = Object.freeze(new EnumSituacaoPneuHelper());