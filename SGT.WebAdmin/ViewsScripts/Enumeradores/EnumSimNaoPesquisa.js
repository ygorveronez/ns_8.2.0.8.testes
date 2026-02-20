var EnumSimNaoPesquisaHelper = function () {
    this.Sim = 1;
    this.Nao = 0;
    this.Todos2 = 2;
    this.Todos = 9;
};

EnumSimNaoPesquisaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.SimNao.Nao, value: this.Nao },
            { text: Localization.Resources.Enumeradores.SimNao.Sim, value: this.Sim }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todos }].concat(this.obterOpcoes());
    },
    obterOpcoesPesquisa2: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todos2 }].concat(this.obterOpcoes());
    }
}

var EnumSimNaoPesquisa = Object.freeze(new EnumSimNaoPesquisaHelper());