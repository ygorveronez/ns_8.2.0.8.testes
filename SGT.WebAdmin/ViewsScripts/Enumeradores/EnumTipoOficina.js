var EnumTipoOficinaHelper = function () {
    this.Interna = 1;
    this.Externa = 2;
};

EnumTipoOficinaHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoOficina.Interna, value: this.Interna },
            { text: Localization.Resources.Enumeradores.TipoOficina.Externa, value: this.Externa }
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoOficina.Todas, value: this.Todas }].concat(this.ObterOpcoes());
    }
};

var EnumTipoOficina = Object.freeze(new EnumTipoOficinaHelper());