var EnumInformacaoCargaPreCargaHelper = function () {
    this.Ambas = 0;
    this.SomenteCargas = 1;
    this.SomentePreCargas = 2;
};

EnumInformacaoCargaPreCargaHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.InformacaoCargaPreCarga.SomenteCargas, value: this.SomenteCargas },
            { text: Localization.Resources.Enumeradores.InformacaoCargaPreCarga.SomentePreCargas, value: this.SomentePreCargas }
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.InformacaoCargaPreCarga.Ambas, value: this.Ambas }].concat(this.ObterOpcoes());
    }
};

var EnumInformacaoCargaPreCarga = Object.freeze(new EnumInformacaoCargaPreCargaHelper());