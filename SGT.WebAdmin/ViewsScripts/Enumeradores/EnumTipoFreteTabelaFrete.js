var EnumTipoFreteTabelaFreteHelper = function () {
    this.Todos = "";
    this.NaoInformado = 0;
    this.Proprio = 1;
    this.Spot = 2;
    this.Terceiro = 3;
}

EnumTipoFreteTabelaFreteHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoFreteTabelaFrete.NaoInformado, value: this.NaoInformado },
            { text: Localization.Resources.Enumeradores.TipoFreteTabelaFrete.Proprio, value: this.Proprio },
            { text: Localization.Resources.Enumeradores.TipoFreteTabelaFrete.Spot, value: this.Spot },
            { text: Localization.Resources.Enumeradores.TipoFreteTabelaFrete.Terceiro, value: this.Terceiro }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoFreteTabelaFrete.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumTipoFreteTabelaFrete = Object.freeze(new EnumTipoFreteTabelaFreteHelper());
