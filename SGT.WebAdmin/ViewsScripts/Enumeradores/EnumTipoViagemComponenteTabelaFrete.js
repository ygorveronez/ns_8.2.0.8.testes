var EnumTipoViagemComponenteTabelaFreteHelper = function () {
    this.Propria = 0;
    this.Terceiros = 1;
};

EnumTipoViagemComponenteTabelaFreteHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoViagemComponenteTabelaFrete.Propria, value: this.Propria.toString() },
            { text: Localization.Resources.Enumeradores.TipoViagemComponenteTabelaFrete.Terceiros, value: this.Terceiros.toString() },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoViagemComponenteTabelaFrete.Todos, value: "" }].concat(this.obterOpcoes());
    }
};

var EnumTipoViagemComponenteTabelaFrete = Object.freeze(new EnumTipoViagemComponenteTabelaFreteHelper());