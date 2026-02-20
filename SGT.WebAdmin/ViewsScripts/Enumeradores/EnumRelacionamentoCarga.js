var EnumRelacionamentoCargaHelper = function () {
    this.Todos = "";
    this.Relacionada = 1;
    this.NaoRelacionada = 2;
};

EnumRelacionamentoCargaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Relacionada", value: this.Relacionada },
            { text: "Não Relacionada", value: this.NaoRelacionada }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumRelacionamentoCarga = Object.freeze(new EnumRelacionamentoCargaHelper());