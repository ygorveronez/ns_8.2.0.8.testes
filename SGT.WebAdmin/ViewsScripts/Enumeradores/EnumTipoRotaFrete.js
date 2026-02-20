var EnumTipoRotaFreteHelper = function () {
    this.Ida = 0;
    this.IdaVolta = 1;
};

EnumTipoRotaFreteHelper.prototype = {
    ObterDescricao: function (tipo) {
        switch (tipo) {
            case this.Ida: return Localization.Resources.Enumeradores.TipoRotaFrete.SomenteIda;
            case this.IdaVolta: return Localization.Resources.Enumeradores.TipoRotaFrete.IdaVolta;
            default: return undefined;
        }
    },
    ObterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoRotaFrete.SomenteIda, value: this.Ida },
            { text: Localization.Resources.Enumeradores.TipoRotaFrete.IdaVolta, value: this.IdaVolta }
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoRotaFrete.Todos, value: "" }].concat(this.ObterOpcoes());
    }
};

var EnumTipoRotaFrete = Object.freeze(new EnumTipoRotaFreteHelper());