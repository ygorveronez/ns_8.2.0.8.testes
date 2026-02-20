var EnumTipoLogAcessoHelper = function () {
    this.Todos = "";
    this.Entrada = 0;
    this.Saida = 1;
};

EnumTipoLogAcessoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Entrada", value: this.Entrada },
            { text: "Saida", value: this.Saida }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoLogAcesso = Object.freeze(new EnumTipoLogAcessoHelper());