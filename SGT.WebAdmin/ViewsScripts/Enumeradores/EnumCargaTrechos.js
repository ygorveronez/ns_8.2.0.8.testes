var EnumCargaTrechosHelper = function () {
    this.Todos = 9;
    this.Nao = 0;
    this.ApenasTrechos = 1;
};

EnumCargaTrechosHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Não", value: this.Nao },
            { text: "Apenas trechos", value: this.ApenasTrechos }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumCargaTrechos = Object.freeze(new EnumCargaTrechosHelper());